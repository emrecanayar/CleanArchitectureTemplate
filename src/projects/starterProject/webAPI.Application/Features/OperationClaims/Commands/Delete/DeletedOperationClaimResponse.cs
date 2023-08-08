using Core.Application.Responses;

namespace Application.Features.OperationClaims.Commands.Delete;

public class DeletedOperationClaimResponse : IResponse
{
    public Guid Id { get; set; }
}
