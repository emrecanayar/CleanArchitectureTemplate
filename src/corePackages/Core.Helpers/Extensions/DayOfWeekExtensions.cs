namespace Core.Helpers.Extensions
{
    public static class DayOfWeekExtensions
    {
        public static bool IsWeekend(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek.IsIn(DayOfWeek.Saturday, DayOfWeek.Sunday);
        }

        public static bool IsWeekday(this DayOfWeek dayOfWeek)
        {
            return dayOfWeek.IsIn(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);
        }

        public static DateTime FindNthWeekDayOfMonth(this DayOfWeek dayOfWeek, int year, int month, int n)
        {
            if (n < 1 || n > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(n));
            }

            var y = 0;

            var daysOfMonth = DateTimeExtensions.DaysOfMonth(year, month);

            var totalInstances = dayOfWeek.TotalInstancesInMonth(year, month);
            if (n == 5 && n > totalInstances)
            {
                n = 4;
            }

            var foundDate = daysOfMonth
                .Where(date => dayOfWeek.Equals(date.DayOfWeek))
                .OrderBy(date => date)
                .Select(x => new { n = ++y, date = x })
                .Where(x => x.n.Equals(n)).Select(x => x.date).First();

            return foundDate;
        }

        public static int TotalInstancesInMonth(this DayOfWeek dayOfWeek, int year, int month)
        {
            return DateTimeExtensions.DaysOfMonth(year, month).Count(date => dayOfWeek.Equals(date.DayOfWeek));
        }

        public static int TotalInstancesInMonth(this DayOfWeek dayOfWeek, DateTime dateTime)
        {
            return dayOfWeek.TotalInstancesInMonth(dateTime.Year, dateTime.Month);
        }
    }
}
