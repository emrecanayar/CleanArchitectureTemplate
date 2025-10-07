using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class UserOperationClaim : Entity<Guid>
    {
        public Guid UserId { get; set; }

        public Guid OperationClaimId { get; set; }

        public virtual User User { get; set; }

        public virtual OperationClaim OperationClaim { get; set; }

        public UserOperationClaim(Guid userId, Guid operationClaimId)
        {
            UserId = userId;
            OperationClaimId = operationClaimId;
            User = default!;
            OperationClaim = default!;
        }

        public UserOperationClaim(Guid id, Guid userId, Guid operationClaimId)
            : base()
        {
            Id = id;
            UserId = userId;
            OperationClaimId = operationClaimId;
            User = default!;
            OperationClaim = default!;
        }
    }
}
