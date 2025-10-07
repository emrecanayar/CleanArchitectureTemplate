using System;
using Application.Features.OperationClaims.Profiles;
using Application.Features.OperationClaims.Rules;
using Application.Services.Repositories;
using Application.Tests.Mocks.FakeData;
using Core.Domain.Entities;
using Core.Test.Application.Repositories;

namespace Application.Tests.Mocks.Repositories
{
    public class OperationClaimMockRepository : BaseMockRepository<IOperationClaimRepository, OperationClaim, Guid, MappingProfiles, OperationClaimBusinessRules, OperationClaimFakeData>
    {
        public OperationClaimMockRepository(OperationClaimFakeData fakeData)
            : base(fakeData)
        {
        }
    }
}
