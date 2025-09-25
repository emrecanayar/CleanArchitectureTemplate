using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class Dictionary : Entity<Guid>
    {
        public Guid LanguageId { get; set; }

        public string EntryKey { get; set; } = string.Empty;

        public string EntryValue { get; set; } = string.Empty;

        public string ValueType { get; set; } = string.Empty;

        public string Entity { get; set; } = string.Empty;

        public string Property { get; set; } = string.Empty;

        public Language Language { get; set; }

        public Dictionary()
        {
            Language = default!;
        }

        public Dictionary(Guid id, Guid languageId, string entryKey, string entryValue, string valueType, string entity, string property)
            : this()
        {
            Id = id;
            LanguageId = languageId;
            EntryKey = entryKey;
            EntryValue = entryValue;
            ValueType = valueType;
            Entity = entity;
            Property = property;
        }
    }
}