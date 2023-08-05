namespace Core.Domain.Dtos
{
    public class LanguageDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Flag { get; set; }
        public ICollection<DictionaryDto> LanguageDictionary { get; set; }

    }
}