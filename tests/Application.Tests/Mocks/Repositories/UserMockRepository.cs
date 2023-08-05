using Application.Features.Users.Profiles;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using Application.Tests.Mocks.FakeData;
using Core.Domain.Entities;
using Core.Test.Application.Repositories;
using System;

namespace Application.Tests.Mocks.Repositories;

public class UserMockRepository : BaseMockRepository<IUserRepository, User, Guid, MappingProfiles, UserBusinessRules, UserFakeData>
{
    public UserMockRepository(UserFakeData fakeData)
        : base(fakeData) { }
}
