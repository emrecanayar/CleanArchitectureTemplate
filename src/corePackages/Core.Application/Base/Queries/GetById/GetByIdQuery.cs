using System.Net;
using AutoMapper;
using Core.Application.Base.Rules;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities.Base;
using Core.Persistence.Dynamic;
using Core.Persistence.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Application.Base.Queries.GetById
{
    public class GetByIdQuery<TEntity, TEntityId, TModel> : IRequest<CustomResponseDto<TModel>>, ISecuredRequest
    where TEntity : Entity<TEntityId>
    where TEntityId : struct, IEquatable<TEntityId>
    where TModel : class
    {
        public TEntityId Id { get; set; }

        public IncludeProperty? IncludeProperty { get; set; }

        public string[] Roles { get; set; }

        public bool RequiresAuthorization { get; set; }

        public GetByIdQuery(string[] roles, bool requiresAuthorization = false)
        {
            Roles = roles ?? Array.Empty<string>();
            RequiresAuthorization = requiresAuthorization;
        }

        public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery<TEntity, TEntityId, TModel>, CustomResponseDto<TModel>>
        {
            private readonly IAsyncRepository<TEntity, TEntityId> _asyncRepository;
            private readonly IMapper _mapper;
            private readonly BaseBusinessRules<TEntity, TEntityId> _baseBusinessRules;

            public GetByIdQueryHandler(IAsyncRepository<TEntity, TEntityId> asyncRepository, IMapper mapper, BaseBusinessRules<TEntity, TEntityId> baseBusinessRules)
            {
                _asyncRepository = asyncRepository;
                _mapper = mapper;
                _baseBusinessRules = baseBusinessRules;
            }

            public async Task<CustomResponseDto<TModel>> Handle(GetByIdQuery<TEntity, TEntityId, TModel> request, CancellationToken cancellationToken)
            {
                await _baseBusinessRules.BaseIdShouldExistWhenSelected(request.Id);
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

                query = query.Where(x => x.Id.Equals(request.Id));
                TEntity? entity = await query.SingleOrDefaultAsync(cancellationToken: cancellationToken);

                TModel mappedTModel = _mapper.Map<TModel>(entity);
                return CustomResponseDto<TModel>.Success((int)HttpStatusCode.OK, mappedTModel, isSuccess: true);
            }
        }
    }
}
