using Core.Domain.Entities.Base;

namespace Core.Domain.Entities
{
    public class Language : Entity<Guid>
    {
        public string Name { get; set; }
        public string Symbol { get; set; }
        public string Flag { get; set; }
        public ICollection<Dictionary> Dictionaries { get; set; }

        public Language()
        {
            Dictionaries = new HashSet<Dictionary>();
        }

        public Language(Guid id, string name, string symbol, string flag) : this()
        {
            Id = id;
            Name = name;
            Symbol = symbol;
            Flag = flag;
        }

    }
}
