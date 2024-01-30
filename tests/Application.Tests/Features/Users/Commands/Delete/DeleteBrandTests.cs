using Application.Features.Users.Commands.Delete;
using Application.Tests.Mocks.FakeData;
using Application.Tests.Mocks.Repositories;
using Core.Application.ResponseTypes.Concrete;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using static Application.Features.Users.Commands.Delete.DeleteUserCommand;

namespace Application.Tests.Features.Users.Commands.Delete;

public class DeleteUserTests : UserMockRepository
{
    private readonly DeleteUserCommandHandler _handler;
    private readonly DeleteUserCommand _command;

    public DeleteUserTests(UserFakeData fakeData, DeleteUserCommand command)
        : base(fakeData)
    {
        _command = command;
        _handler = new DeleteUserCommandHandler(MockRepository.Object, Mapper, BusinessRules);
    }

    [Fact]
    public async Task DeleteShouldSuccessfully()
    {
        _command.Id = Guid.Parse("e16d144a-8684-4f28-8d24-e816a560dfb3");
        CustomResponseDto<DeletedUserResponse> result = await _handler.Handle(_command, CancellationToken.None);
        Assert.NotNull(result);
    }

}
