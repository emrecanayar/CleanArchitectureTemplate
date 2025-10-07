namespace Core.Domain.Entities.Base
{
    public interface IHasTimestamps
    {
        DateTime CreatedDate { get; set; }

        DateTime? ModifiedDate { get; set; }

        DateTime? DeletedDate { get; set; }

        bool IsDeleted { get; set; }
    }
}
