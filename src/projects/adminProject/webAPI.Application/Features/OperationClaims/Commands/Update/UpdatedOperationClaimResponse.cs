using Core.Application.Responses;

namespace webAPI.Application.Features.OperationClaims.Commands.Update
{
    public class UpdatedOperationClaimResponse : IResponse
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public UpdatedOperationClaimResponse()
        {
            Name = string.Empty;
        }

        public UpdatedOperationClaimResponse(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
