using System.Globalization;
using Core.Domain.ComplexTypes.Enums;
using Core.Domain.Dtos;
using Core.Domain.Entities;
using Core.Persistence.Constants;
using Core.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Core.Persistence.Repositories
{
    public class CustomStringLocalizer
    {
        private static CultureInfo? _culture = CultureInfo.CurrentCulture;

        public CultureInfo GetCulture() => _culture ?? default!;

        public static void SetCulture(string symbol)
        {
            _culture = new CultureInfo(symbol);
        }

        public static string L(string name)
        {
            var culture = _culture ?? CultureInfo.CurrentCulture;
            string? translation = string.Empty;
            using var db = createDbContext();
            translation = db.Set<Dictionary>().Where(x => x.Language.Symbol == culture.Name
               && x.EntryKey == name && x.Status == RecordStatu.Active).AsNoTracking().FirstOrDefault()?.EntryValue;
            return translation ?? string.Empty;
        }
        public static List<DictionaryDto> GetValues(string name, bool getAllTranslations)
        {

            using var db = createDbContext();
            List<Dictionary> dictionaries;
            if (getAllTranslations)
            {
                dictionaries = db.Set<Dictionary>().Include(nameof(Dictionary.Language)).Where(x => x.EntryKey == name).ToList();
            }
            else
            {
                var culture = _culture ?? CultureInfo.CurrentCulture;
                dictionaries = db.Set<Dictionary>().Include(nameof(Dictionary.Language)).Where(x => x.Language.Symbol == culture.Name && x.EntryKey == name).ToList();
            }
            var dictionariesDtos = dictionaries.Select(x => new DictionaryDto()
            {
                Id = x.Id,
                Entity = x.Entity,
                EntryKey = x.EntryKey,
                EntryValue = x.EntryValue,
                LanguageId = x.LanguageId,
                Property = x.Property,
                ValueType = x.ValueType
            });
            return dictionariesDtos.ToList();

        }

        public string this[string name]
        {
            get
            {
                var culture = _culture ?? CultureInfo.CurrentCulture;
                string? translation = string.Empty;
                using var db = createDbContext();
                translation = db.Set<Dictionary>().Where(x => x.Language.Symbol == culture.Name
                   && x.EntryKey == name && x.Status == RecordStatu.Active).AsNoTracking().FirstOrDefault()?.EntryValue;
                return translation ?? string.Empty;
            }
        }

        public List<DictionaryDto> this[string name, bool getAllTranslations]
        {
            get
            {
                using var db = createDbContext();
                List<Dictionary> dictionaries;
                if (getAllTranslations)
                {
                    dictionaries = db.Set<Dictionary>().Include(nameof(Dictionary.Language)).Where(x => x.EntryKey == name).ToList();
                }
                else
                {
                    var culture = _culture ?? CultureInfo.CurrentCulture;
                    dictionaries = db.Set<Dictionary>().Include(nameof(Dictionary.Language)).Where(x => x.Language.Symbol == culture.Name && x.EntryKey == name).ToList();
                }
                var dictionariesDtos = dictionaries.Select(x => new DictionaryDto()
                {
                    Id = x.Id,
                    Entity = x.Entity,
                    EntryKey = x.EntryKey,
                    EntryValue = x.EntryValue,
                    LanguageId = x.LanguageId,
                    Property = x.Property,
                    ValueType = x.ValueType
                });
                return dictionariesDtos.ToList();
            }
        }

        public static LanguageDto GetCurrentLanguage()
        {
            CultureInfo culture = _culture ?? CultureInfo.CurrentCulture;

            using LocalizationDbContext db = createDbContext();
            Language? lang = db.Set<Language>().Where(x => x.Status == RecordStatu.Active && x.Symbol == culture.Name).AsNoTracking().FirstOrDefault();

            if (lang == null)
            {
                throw new InvalidOperationException("No matching language found.");
            }

            return new LanguageDto
            {
                Id = lang.Id,
                Name = lang.Name,
                Flag = lang.Flag,
                Symbol = lang.Symbol,
            };
        }

        public static List<Language> GetActiveLanguages()
        {
            using LocalizationDbContext db = createDbContext();
            List<Language> langs = db.Set<Language>().AsNoTracking().ToList();
            return langs;
        }

        public static async Task<List<Language>> GetActiveLanguagesAsync()
        {
            using LocalizationDbContext db = createDbContext();
            List<Language> langs = await db.Set<Language>().Where(x => x.Status == RecordStatu.Active).AsNoTracking().ToListAsync();
            return langs;
        }

        public string? GetUserCulture(Guid userId)
        {
            using LocalizationDbContext db = createDbContext();
            User? user = db.Set<User>().Where(x => x.Status == RecordStatu.Active).AsNoTracking().FirstOrDefault();

            if (user == null)
            {
                throw new InvalidOperationException("No matching user found.");
            }

            return Enum.GetName(user.CultureType);
        }

        private static LocalizationDbContext createDbContext()
        {
            var builder = new DbContextOptionsBuilder<LocalizationDbContext>();
            builder.UseSqlServer(LocalizationConnectionString.Execute());
            return new LocalizationDbContext(builder.Options);
        }
    }
}