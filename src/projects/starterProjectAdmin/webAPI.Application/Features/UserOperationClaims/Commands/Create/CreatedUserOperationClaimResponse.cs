using Core.Application.Responses;

namespace webAPI.Application.Features.UserOperationClaims.Commands.Create
{
    public class CreatedUserOperationClaimResponse : IResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid OperationClaimId { get; set; }
    }
}
