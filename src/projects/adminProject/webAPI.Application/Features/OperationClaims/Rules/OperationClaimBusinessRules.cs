using Core.Application.Rules;
using Core.CrossCuttingConcerns.Exceptions.Types;
using Core.Domain.Entities;
using webAPI.Application.Features.OperationClaims.Constants;
using webAPI.Application.Services.Repositories;

namespace webAPI.Application.Features.OperationClaims.Rules
{
    public class OperationClaimBusinessRules : BaseBusinessRules
    {
        private readonly IOperationClaimRepository _operationClaimRepository;

        public OperationClaimBusinessRules(IOperationClaimRepository operationClaimRepository)
        {
            _operationClaimRepository = operationClaimRepository;
        }

        public Task OperationClaimShouldExistWhenSelected(OperationClaim? operationClaim)
        {
            if (operationClaim == null)
            {
                throw new BusinessException(OperationClaimsMessages.NotExists);
            }

            return Task.CompletedTask;
        }

        public async Task OperationClaimIdShouldExistWhenSelected(Guid id)
        {
            bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id == id, enableTracking: false);
            if (doesExist)
            {
                throw new BusinessException(OperationClaimsMessages.NotExists);
            }
        }

        public async Task OperationClaimNameShouldNotExistWhenCreating(string name)
        {
            bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Name == name, enableTracking: false);
            if (doesExist)
            {
                throw new BusinessException(OperationClaimsMessages.AlreadyExists);
            }
        }

        public async Task OperationClaimNameShouldNotExistWhenUpdating(Guid id, string name)
        {
            bool doesExist = await _operationClaimRepository.AnyAsync(predicate: b => b.Id != id && b.Name == name, enableTracking: false);
            if (doesExist)
            {
                throw new BusinessException(OperationClaimsMessages.AlreadyExists);
            }
        }
    }
}
