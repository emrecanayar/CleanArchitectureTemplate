using Core.Application.Responses;

namespace webAPI.Application.Features.OperationClaims.Commands.Delete
{
    public class DeletedOperationClaimResponse : IResponse
    {
        public Guid Id { get; set; }
    }
}
