using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Helpers.Helpers;
using MediatR;
using System.Net;

namespace Core.Application.Base.Queries.GetListNavigationProperty
{
    public class GetListNavigationPropertyQuery<TEntity> : IRequest<CustomResponseDto<List<string>>>, ISecuredRequest
        where TEntity : class
    {
        public string[] Roles { get; set; }
        public bool RequiresAuthorization { get; set; }

        public GetListNavigationPropertyQuery(string[] roles = null, bool requiresAuthorization = false)
        {
            Roles = roles ?? Array.Empty<string>();
            RequiresAuthorization = requiresAuthorization;
        }

        public class GetListNavigationPropertyQueryHandler<TEntity> : IRequestHandler<GetListNavigationPropertyQuery<TEntity>, CustomResponseDto<List<string>>>
            where TEntity : class
        {
            public Task<CustomResponseDto<List<string>>> Handle(GetListNavigationPropertyQuery<TEntity> request, CancellationToken cancellationToken)
            {
                List<string> navigationProperties = ReflectionHelper.GetNavigationPropertyNames(typeof(TEntity));
                return Task.FromResult(CustomResponseDto<List<string>>.Success((int)HttpStatusCode.OK, navigationProperties, true));
            }
        }
    }
}
