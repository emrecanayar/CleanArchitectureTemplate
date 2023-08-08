using Core.Application.Responses;

namespace Application.Features.OperationClaims.Queries.GetById;

public class GetByIdOperationClaimResponse : IResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public GetByIdOperationClaimResponse()
    {
        Name = string.Empty;
    }

    public GetByIdOperationClaimResponse(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}
