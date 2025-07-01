using Core.Domain.ComplexTypes.Enums;

namespace Core.Domain.Entities.Base
{
    public abstract class Entity : IEntity<Guid>, ISoftDelete, IAuditable, IHasTimestamps
    {
        public Guid Id { get; set; }
        public RecordStatu? Status { get; set; } = RecordStatu.Active;
        public string CreatedBy { get; set; } = "Admin";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
    public abstract class Entity<TKey> : IEntity<TKey>, ISoftDelete, IAuditable, IHasTimestamps
       where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        public RecordStatu? Status { get; set; } = RecordStatu.Active;
        public string CreatedBy { get; set; } = "Admin";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string ModifiedBy { get; set; } = string.Empty;
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedDate { get; set; }

        protected Entity()
        {
            Id = default!;
            ModifiedBy = default!;
        }

        protected Entity(TKey id)
        {
            Id = id;
        }
    }
}

