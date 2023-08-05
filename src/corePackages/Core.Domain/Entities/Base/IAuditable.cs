namespace Core.Domain.Entities.Base
{
    public interface IAuditable
    {
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
    }
}
