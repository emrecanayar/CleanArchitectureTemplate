using System.Threading;
using System.Threading.Tasks;
using Application.Features.OperationClaims.Commands.Create;
using Application.Tests.Mocks.FakeData;
using Application.Tests.Mocks.Repositories;
using Core.Application.ResponseTypes.Concrete;
using Xunit;
using static Application.Features.OperationClaims.Commands.Create.CreateOperationClaimCommand;

namespace Application.Tests.Features.OperationClaims.Commands.Create
{
    public class CreateOperationClaimTests : OperationClaimMockRepository
    {
        private readonly CreateOperationClaimCommandValidator _validator;
        private readonly CreateOperationClaimCommand _command;
        private readonly CreateOperationClaimCommandHandler _handler;

        public CreateOperationClaimTests(OperationClaimFakeData fakeData, CreateOperationClaimCommandValidator validator, CreateOperationClaimCommand command)
            : base(fakeData)
        {
            _validator = validator;
            _command = command;
            _handler = new CreateOperationClaimCommandHandler(_mockRepository.Object, _mapper, _businessRules);
        }

        [Fact]
        public async Task CreateShouldSuccessfully()
        {
            _command.Name = "Editor";
            CustomResponseDto<CreatedOperationClaimResponse> result = await _handler.Handle(_command, CancellationToken.None);

            Assert.Equal(expected: "Editor", result.Data.Name);
        }
    }
}
