namespace Core.Domain.Entities.Base
{
    public interface IConfirmationEntity
    {
        public int Id { get; set; }
        public bool Suspended { get; set; }
    }
}