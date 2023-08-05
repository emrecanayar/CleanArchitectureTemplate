using Core.Domain.Entities.Base;

namespace Core.Domain.Dtos
{
    public class MultiLanguageDto<TEntity> where TEntity : class, IDto, new()
    {
        public MultiLanguageDto()
        {
            Entity = new();
            Translations = new();
        }
        public TEntity Entity { get; set; }
        public List<DictionaryDto> Translations { get; set; }
    }
}
