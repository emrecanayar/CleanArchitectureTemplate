using System.Net;
using Core.Application.Base.Rules;
using Core.Application.Pipelines.Authorization;
using Core.Application.ResponseTypes.Concrete;
using Core.Domain.Entities.Base;
using Core.Persistence.Repositories;
using MediatR;

namespace Core.Application.Base.Commands.Delete
{
    public class DeleteCommand<TEntity, TEntityId, TModel> : IRequest<CustomResponseDto<bool>>, ISecuredRequest
       where TEntity : Entity<TEntityId>
       where TEntityId : struct, IEquatable<TEntityId>
       where TModel : IEntityModel<TEntityId>
    {
        public TModel Model { get; set; }

        public string[] Roles { get; set; }

        public bool RequiresAuthorization { get; set; }

        public DeleteCommand(TModel model, string[] roles, bool requiresAuthorization = false)
        {
            Model = model;
            Roles = roles ?? Array.Empty<string>();
            RequiresAuthorization = requiresAuthorization;
        }

        public class DeleteCommandHandler : IRequestHandler<DeleteCommand<TEntity, TEntityId, TModel>, CustomResponseDto<bool>>
        {
            private readonly IAsyncRepository<TEntity, TEntityId> _asyncRepository;
            private readonly BaseBusinessRules<TEntity, TEntityId> _baseBusinessRules;

            public DeleteCommandHandler(IAsyncRepository<TEntity, TEntityId> asyncRepository, BaseBusinessRules<TEntity, TEntityId> baseBusinessRules)
            {
                _asyncRepository = asyncRepository;
                _baseBusinessRules = baseBusinessRules;
            }

            public async Task<CustomResponseDto<bool>> Handle(DeleteCommand<TEntity, TEntityId, TModel> request, CancellationToken cancellationToken)
            {
                await _baseBusinessRules.BaseIdShouldExistWhenSelected(request.Model.Id);
                TEntity? entity = await _asyncRepository.GetAsync(x => x.Id.Equals(request.Model.Id));
                await _asyncRepository.DeleteAsync(entity!);

                return CustomResponseDto<bool>.Success((int)HttpStatusCode.OK, true, isSuccess: true);
            }
        }
    }
}
