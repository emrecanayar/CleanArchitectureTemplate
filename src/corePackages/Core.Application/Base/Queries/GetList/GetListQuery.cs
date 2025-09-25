using System.Net;
using AutoMapper;
using Core.Application.Base.Rules;
using Core.Application.Pipelines.Authorization;
using Core.Application.Requests;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities.Base;
using Core.Persistence.Dynamic;
using Core.Persistence.Paging;
using Core.Persistence.Repositories;
using MediatR;

namespace Core.Application.Base.Queries.GetList
{
    public class GetListQuery<TEntity, TEntityId, TModel> : IRequest<CustomResponseDto<TModel>>, ISecuredRequest
     where TEntity : Entity<TEntityId>
     where TEntityId : struct, IEquatable<TEntityId>
     where TModel : BasePageableModel
    {
        public PageRequest PageRequest { get; set; }

        public IncludeProperty? IncludeProperty { get; set; }

        public string[] Roles { get; set; }

        public bool RequiresAuthorization { get; set; }

        public GetListQuery(string[] roles, bool requiresAuthorization = false)
        {
            Roles = roles ?? Array.Empty<string>();
            RequiresAuthorization = requiresAuthorization;
        }

        public class GetListQueryHandler : IRequestHandler<GetListQuery<TEntity, TEntityId, TModel>, CustomResponseDto<TModel>>
        {
            private readonly IRepository<TEntity, TEntityId> _repository;
            private readonly IAsyncRepository<TEntity, TEntityId> _asyncRepository;
            private readonly IMapper _mapper;
            private readonly BaseBusinessRules<TEntity, TEntityId> _baseBusinessRules;

            public GetListQueryHandler(IRepository<TEntity, TEntityId> repository, IAsyncRepository<TEntity, TEntityId> asyncRepository, IMapper mapper, BaseBusinessRules<TEntity, TEntityId> baseBusinessRules)
            {
                _repository = repository;
                _asyncRepository = asyncRepository;
                _mapper = mapper;
                _baseBusinessRules = baseBusinessRules;
            }

            public async Task<CustomResponseDto<TModel>> Handle(GetListQuery<TEntity, TEntityId, TModel> request, CancellationToken cancellationToken)
            {
                IQueryable<TEntity> query = _asyncRepository.Query();

                if (request.IncludeProperty?.IncludeProperties != null)
                {
                    StringIncludeSpecification<TEntity> includeSpecification = new StringIncludeSpecification<TEntity>();
                    foreach (string includeProperty in request.IncludeProperty.IncludeProperties)
                    {
                        includeSpecification.Include(includeProperty);
                    }

                    query = includeSpecification.ApplyIncludes(query);
                }

                IPaginate<TEntity> entities = await query.ToPaginateAsync(index: request.PageRequest.PageIndex, size: request.PageRequest.PageSize);
                TModel mappedTModel = _mapper.Map<TModel>(entities);
                return CustomResponseDto<TModel>.Success((int)HttpStatusCode.OK, mappedTModel, isSuccess: true);
            }
        }
    }
}