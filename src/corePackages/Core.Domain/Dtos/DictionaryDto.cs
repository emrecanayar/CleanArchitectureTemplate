namespace Core.Domain.Dtos
{
    public class DictionaryDto
    {
        public Guid Id { get; set; }
        public Guid LanguageId { get; set; }
        public string EntryKey { get; set; }
        public string EntryValue { get; set; }
        public string ValueType { get; set; }
        public string Entity { get; set; }
        public string Property { get; set; }
        public LanguageDto Language { get; set; }

    }
}