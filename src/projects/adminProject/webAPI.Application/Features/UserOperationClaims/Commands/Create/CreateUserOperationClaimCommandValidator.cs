using FluentValidation;

namespace webAPI.Application.Features.UserOperationClaims.Commands.Create
{
    public class CreateUserOperationClaimCommandValidator : AbstractValidator<CreateUserOperationClaimCommand>
    {
        public CreateUserOperationClaimCommandValidator()
        {
        }
    }
}
