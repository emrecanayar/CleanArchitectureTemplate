using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Core.Application.Pipelines.Security;
using Core.BackgroundJob.Extensions;
using Core.BackgroundJob.Services;
using Core.CrossCuttingConcerns.Exceptions.Extensions;
using Core.Persistence;
using Core.Security;
using Core.Security.Encryption;
using Core.Security.JWT;
using Core.Utilities.ApiDoc;
using Core.Utilities.Messages;
using Hangfire;
using Hangfire.SqlServer;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using webAPI.Extensions;
using webAPI.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;

string? connectionString = builder.Configuration.GetConnectionString("ConnectionString");
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecryptedJsonModelBinderProvider());
})
 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
 .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSecurityServices();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks().AddSqlServer(connectionString!);
if (string.Equals(environment, "Production"))
{
    builder.Services.AddHangfireServices();
    builder.Services.AddHangfireServer(options =>
    {
        options.WorkerCount = Environment.ProcessorCount * 2;
    });
    builder.Services.AddHangfire(config =>
    {
        config.UseStorage(new SqlServerStorage(connectionString, new SqlServerStorageOptions
        {
            SchemaName = "Hangfire", // Varsayýlan olarak 'Hangfire' þemasý
            QueuePollInterval = TimeSpan.FromSeconds(15), // Kuyruk tarama aralýðý
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5), // Maksimum komut batch süresi
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5), // Kaybolan iþler için kaydýrma süresi
            DashboardJobListLimit = 5000, // Dashboard'da gösterilecek iþ sayýsý
        }));

        if (string.Equals(environment, "Production"))
        {
            config.ConfigureRecurringJobs();
        }
    });
}

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepositoryModule()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", new CorsPolicyBuilder()
        .AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader()
        .Build());
});

TokenOptions? tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<TokenOptions>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = tokenOptions!.Issuer,
        ValidAudience = tokenOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey),
    };
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.CustomSchemaIds(type => type.ToString());
    opt.SwaggerDoc(ProjectSwaggerMessages.Version, new OpenApiInfo
    {
        Version = ProjectSwaggerMessages.Version,
        Title = ProjectSwaggerMessages.Title,
        Description = ProjectSwaggerMessages.Description,
    });
    opt.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
            ? $"{methodInfo!.DeclaringType!.Name}.{methodInfo.Name}"
            : default(Guid).ToString();
    });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345.54321\"",
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
                { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        },
    });
    opt.OperationFilter<AddAuthHeaderOperationFilter>();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.UseStaticFiles();

app.UseSwagger(x =>
{
    x.SerializeAsV2 = true;
});
app.UseSwaggerUI(options =>
{
    options.DocExpansion(DocExpansion.None);
    options.DefaultModelExpandDepth(-1);
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
    options.InjectStylesheet("/swagger-custom/swagger-custom-styles.css");
    options.InjectJavascript("/swagger-custom/swagger-custom-script.js", "text/javascript");
});

app.UseCors("AllowAll");
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.ConfigureCustomExceptionMiddleware();
app.UseResponseCompression();

if (string.Equals(environment, "Production"))
{
    var retryPolicyService = app.Services.GetRequiredService<IRetryPolicyService>();
    retryPolicyService.ApplyPolicy();

    app.UseHangfireDashboard("/job", new DashboardOptions
    {
        DashboardTitle = "Project Hangfire DashBoard",
        AppPath = "/Home/HangfireAbout",
    });
}

app.MapControllers();
app.UseHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
});

app.UseHealthChecksUI(config =>
{
    config.UIPath = "/monitor"; // Arayüz adresi
});
app.Run();