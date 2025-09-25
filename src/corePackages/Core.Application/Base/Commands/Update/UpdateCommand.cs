using System.Net;
using AutoMapper;
using Core.Application.Base.Rules;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities.Base;
using Core.Persistence.Repositories;
using MediatR;

namespace Core.Application.Base.Commands.Update
{
    public class UpdateCommand<TEntity, TEntityId, TModel> : IRequest<CustomResponseDto<TModel>>, ISecuredRequest
     where TEntity : Entity<TEntityId>
     where TEntityId : struct, IEquatable<TEntityId>
     where TModel : IEntityModel<TEntityId>
    {
        public TModel Model { get; set; }

        public string[] Roles { get; set; }

        public bool RequiresAuthorization { get; set; }

        public UpdateCommand(TModel model, string[] roles, bool requiresAuthorization = false)
        {
            Model = model;
            Roles = roles ?? Array.Empty<string>();
            RequiresAuthorization = requiresAuthorization;
        }

        public class UpdateCommandHandler : IRequestHandler<UpdateCommand<TEntity, TEntityId, TModel>, CustomResponseDto<TModel>>
        {
            private readonly IAsyncRepository<TEntity, TEntityId> _asyncRepository;
            private readonly IMapper _mapper;
            private readonly BaseBusinessRules<TEntity, TEntityId> _baseBusinessRules;

            public UpdateCommandHandler(IAsyncRepository<TEntity, TEntityId> asyncRepository, IMapper mapper, BaseBusinessRules<TEntity, TEntityId> baseBusinessRules)
            {
                _asyncRepository = asyncRepository;
                _mapper = mapper;
                _baseBusinessRules = baseBusinessRules;
            }

            public async Task<CustomResponseDto<TModel>> Handle(UpdateCommand<TEntity, TEntityId, TModel> request, CancellationToken cancellationToken)
            {
                await _baseBusinessRules.BaseIdShouldExistWhenSelected(request.Model.Id);
                TEntity? entity = await _asyncRepository.GetAsync(x => x.Id.Equals(request.Model.Id));

                _mapper.Map(request.Model, entity);
                TEntity updatedEntity = await _asyncRepository.UpdateAsync(entity!);
                TModel updatedModel = _mapper.Map<TModel>(updatedEntity);

                return CustomResponseDto<TModel>.Success((int)HttpStatusCode.OK, updatedModel, isSuccess: true);
            }
        }
    }
}