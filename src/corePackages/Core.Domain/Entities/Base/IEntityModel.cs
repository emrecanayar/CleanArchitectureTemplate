namespace Core.Domain.Entities.Base
{
    public interface IEntityModel<TEntityId>
    {
        TEntityId Id { get; set; }
    }
}
