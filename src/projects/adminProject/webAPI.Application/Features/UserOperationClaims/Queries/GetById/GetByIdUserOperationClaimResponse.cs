using Core.Application.Responses;

namespace webAPI.Application.Features.UserOperationClaims.Queries.GetById
{
    public class GetByIdUserOperationClaimResponse : IResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid OperationClaimId { get; set; }
    }
}
