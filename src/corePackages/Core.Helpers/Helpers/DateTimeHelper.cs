using System.Globalization;

namespace Core.Helpers.Helpers
{
    public static class DateTimeHelper
    {
        private static Calendar _cal = new GregorianCalendar();

        public static DateTime? AddDays(DateTime time, int days)
        {
            try
            {
                return _cal.AddDays(time, days);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? AddMonths(DateTime time, int months)
        {
            try
            {
                return _cal.AddMonths(time, months);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? AddYears(DateTime time, int years)
        {
            try
            {
                return _cal.AddYears(time, years);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        public static DateTime? SetYear(DateTime date, int year)
        {
            return DateTimeHelper.AddYears(date, year - date.Year);
        }

        public static DateTime? SetYearMonth(DateTime date, DateTime yearMonth)
        {
            DateTime? target = SetYear(date, yearMonth.Year);
            if (target.HasValue)
            {
                target = DateTimeHelper.AddMonths(target.Value, yearMonth.Month - date.Month);
            }

            return target;
        }

        public static int CompareDays(DateTime dt1, DateTime dt2)
        {
            var dt1DiscardedTime = DiscardTime(dt1);
            var dt2DiscardedTime = DiscardTime(dt2);

            if (!dt1DiscardedTime.HasValue || !dt2DiscardedTime.HasValue)
            {
                throw new InvalidOperationException("Unable to discard the time from the date.");
            }

            return DateTime.Compare(dt1DiscardedTime.Value, dt2DiscardedTime.Value);
        }

        public static int CompareYearMonth(DateTime dt1, DateTime dt2)
        {
            return ((dt1.Year - dt2.Year) * 12) + (dt1.Month - dt2.Month);
        }

        public static int DecadeOfDate(DateTime date)
        {
            return date.Year - (date.Year % 10);
        }

        public static DateTime DiscardDayTime(DateTime d)
        {
            return new DateTime(d.Year, d.Month, 1, 0, 0, 0, d.Kind);
        }

        public static DateTime? DiscardTime(DateTime? d)
        {
            if (d == null)
            {
                return null;
            }

            return d.Value.Date;
        }

        public static int EndOfDecade(DateTime date)
        {
            return DecadeOfDate(date) + 9;
        }

        public static DateTimeFormatInfo GetCurrentDateFormat()
        {
            return GetDateFormat(CultureInfo.CurrentCulture);
        }

        public static DateTimeFormatInfo GetDateFormat(CultureInfo culture)
        {
            if (culture.Calendar is GregorianCalendar)
            {
                return culture.DateTimeFormat;
            }
            else
            {
                GregorianCalendar? foundCal = null;
                DateTimeFormatInfo? dtfi = null;

                var optionalCalendars = culture.OptionalCalendars.OfType<GregorianCalendar>().ToList();

                foundCal = optionalCalendars.Find(cal => cal.CalendarType == GregorianCalendarTypes.Localized)
                    ?? optionalCalendars.FirstOrDefault();

                if (foundCal == null)
                {
                    // if there are no GregorianCalendars in the OptionalCalendars list, use the invariant dtfi
                    dtfi = ((CultureInfo)CultureInfo.InvariantCulture.Clone()).DateTimeFormat;
                    dtfi.Calendar = new GregorianCalendar();
                }
                else
                {
                    dtfi = ((CultureInfo)culture.Clone()).DateTimeFormat;
                    dtfi.Calendar = foundCal;
                }

                return dtfi;
            }
        }

        public static bool InRange(DateTime date, DateTime start, DateTime end)
        {
            if (CompareDays(date, start) > -1 && CompareDays(date, end) < 1)
            {
                return true;
            }

            return false;
        }

        public static string ToDayString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = date.Value.Day.ToString(format);
            }

            return result;
        }

        public static string ToYearMonthPatternString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = date.Value.ToString(format.YearMonthPattern, format);
            }

            return result;
        }

        public static string ToYearString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = date.Value.Year.ToString(format);
            }

            return result;
        }

        public static string ToAbbreviatedMonthString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                string[] monthNames = format.AbbreviatedMonthNames;
                if (monthNames != null && monthNames.Length > 0)
                {
                    result = monthNames[(date.Value.Month - 1) % monthNames.Length];
                }
            }

            return result;
        }

        public static string ToLongDateString(DateTime? date, CultureInfo culture)
        {
            string result = string.Empty;
            DateTimeFormatInfo format = GetDateFormat(culture);

            if (date.HasValue && format != null)
            {
                result = date.Value.Date.ToString(format.LongDatePattern, format);
            }

            return result;
        }
    }
}
