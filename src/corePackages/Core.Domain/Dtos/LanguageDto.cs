namespace Core.Domain.Dtos
{
    public class LanguageDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Symbol { get; set; }
        public required string Flag { get; set; }
        public required ICollection<DictionaryDto> LanguageDictionary { get; set; }

    }
}