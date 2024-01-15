namespace Core.Domain.Dtos
{
    public class DictionaryDto
    {
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public required string EntryKey { get; set; }
        public required string EntryValue { get; set; }
        public required string ValueType { get; set; }
        public required string Entity { get; set; }
        public required string Property { get; set; }
        public LanguageDto? Language { get; set; }

    }
}