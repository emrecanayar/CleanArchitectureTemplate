using Core.Application.Responses;

namespace webAPI.Application.Features.UserOperationClaims.Commands.Delete
{
    public class DeletedUserOperationClaimResponse : IResponse
    {
        public Guid Id { get; set; }
    }
}
