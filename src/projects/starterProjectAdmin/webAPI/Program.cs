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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using webAPI.Application;
using webAPI.Persistence.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecryptedJsonModelBinderProvider());
})
 .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase)
 .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddSecurityServices();
builder.Services.AddApplicationServices();
builder.Services.AddHangfireServices();
builder.Services.AddHttpContextAccessor();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder => containerBuilder.RegisterModule(new RepositoryModule()));


builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

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
        ValidIssuer = tokenOptions.Issuer,
        ValidAudience = tokenOptions.Audience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
    };
});

builder.Services.AddSwaggerGen(opt =>
{
    opt.CustomSchemaIds(type => type.ToString());
    opt.SwaggerDoc(ProjectSwaggerMessages.Version, new OpenApiInfo
    {
        Version = ProjectSwaggerMessages.Version,
        Title = ProjectSwaggerMessages.Title,
        Description = ProjectSwaggerMessages.Description
    });
    opt.CustomOperationIds(apiDesc =>
    {
        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
            ? $"{methodInfo.DeclaringType.Name}.{methodInfo.Name}"
            : new Guid().ToString();
    });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345.54321\""
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
                { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        }
    });
    opt.OperationFilter<AddAuthHeaderOperationFilter>();
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
});


var app = builder.Build();

app.UseStaticFiles();

var retryPolicyService = app.Services.GetRequiredService<IRetryPolicyService>();
retryPolicyService.ApplyPolicy();

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
//app.UseMiddleware<DecryptionMiddleware>();
app.ConfigureCustomExceptionMiddleware();

app.UseHangfireServer();
app.UseHangfireDashboard("/job", new DashboardOptions
{
    DashboardTitle = "Project Hangfire DashBoard",
    AppPath = "/Home/HangfireAbout",

});

app.MapControllers();

app.Run();