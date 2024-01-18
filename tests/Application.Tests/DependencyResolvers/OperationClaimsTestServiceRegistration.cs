using Application.Features.OperationClaims.Commands.Create;
using Application.Tests.Mocks.FakeData;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Tests.DependencyResolvers
{
    public static class OperationClaimsTestServiceRegistration
    {
        public static void AddOperationClaimsServices(this IServiceCollection services)
        {
            services.AddTransient<OperationClaimFakeData>();
            services.AddTransient<CreateOperationClaimCommand>();
            services.AddSingleton<CreateOperationClaimCommandValidator>();
        }
    }
}
