namespace Core.Domain.Entities.Base
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }
}
