using System.Text;
using System.Xml;
using Core.Helpers.Extensions;

namespace Core.Timing
{
    /// <summary>
    /// A helper class for timezone operations.
    /// </summary>
    public static class TimezoneHelper
    {
        private static readonly Dictionary<string, string> _windowsTimeZoneMappings = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> _ianaTimeZoneMappings = new Dictionary<string, string>();
        private static readonly object _syncObj = new object();

        public static string WindowsToIana(string windowsTimezoneId)
        {
            if (windowsTimezoneId.Equals("UTC", StringComparison.OrdinalIgnoreCase))
            {
                return "Etc/UTC";
            }

            GetTimezoneMappings();

            if (_windowsTimeZoneMappings.ContainsKey(windowsTimezoneId))
            {
                return _windowsTimeZoneMappings[windowsTimezoneId];
            }

            throw new InvalidOperationException($"Unable to map {windowsTimezoneId} to iana timezone.");
        }

        public static string IanaToWindows(string ianaTimezoneId)
        {
            if (ianaTimezoneId.Equals("Etc/UTC", StringComparison.OrdinalIgnoreCase))
            {
                return "UTC";
            }

            GetTimezoneMappings();

            if (_ianaTimeZoneMappings.ContainsKey(ianaTimezoneId))
            {
                return _ianaTimeZoneMappings[ianaTimezoneId];
            }

            throw new InvalidOperationException(string.Format("Unable to map {0} to windows timezone.", ianaTimezoneId));
        }

        public static DateTime? Convert(DateTime? date, string fromTimeZoneId, string toTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = FindTimeZoneInfo(fromTimeZoneId);
            var destinationTimeZone = FindTimeZoneInfo(toTimeZoneId);

            return TimeZoneInfo.ConvertTime(date.Value, sourceTimeZone!, destinationTimeZone!);
        }

        public static DateTime? ConvertFromUtc(DateTime? date, string toTimeZoneId)
        {
            return Convert(date, "UTC", toTimeZoneId);
        }

        public static DateTimeOffset? ConvertFromUtcToDateTimeOffset(DateTime? date, string timeZoneId)
        {
            var zonedDate = ConvertFromUtc(date, timeZoneId);

            return ConvertToDateTimeOffset(zonedDate, timeZoneId);
        }

        public static DateTimeOffset? ConvertToDateTimeOffset(DateTime? date, string timeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return ConvertToDateTimeOffset(date.Value, timeZoneId);
        }

        public static DateTimeOffset ConvertToDateTimeOffset(DateTime date, string timeZoneId)
        {
            var timeZone = FindTimeZoneInfo(timeZoneId);
            TimeSpan offset = timeZone!.BaseUtcOffset;
            var rule = timeZone.GetAdjustmentRules().FirstOrDefault(x => date >= x.DateStart && date <= x.DateEnd);

            if (!timeZone.SupportsDaylightSavingTime || rule == null)
            {
                return new DateTimeOffset(date, offset);
            }

            var daylightStart = GetDaylightTransition(date, rule.DaylightTransitionStart);
            var daylightEnd = GetDaylightTransition(date, rule.DaylightTransitionEnd);

            if (date >= daylightStart && date <= daylightEnd)
            {
                offset = offset.Add(rule.DaylightDelta);
            }

            return new DateTimeOffset(date, offset);
        }

        private static DateTime GetDaylightTransition(DateTime date, TimeZoneInfo.TransitionTime transitionTime)
        {
            var daylightTime = new DateTime(date.Year, transitionTime.Month, 1, 0, 0, 0, DateTimeKind.Local); // Assuming local time

            if (transitionTime.IsFixedDateRule)
            {
                daylightTime = new DateTime(daylightTime.Year, daylightTime.Month, transitionTime.Day, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second, DateTimeKind.Local); // Assuming local time
            }
            else
            {
                daylightTime = daylightTime.NthOf(transitionTime.Week, transitionTime.DayOfWeek);
            }

            daylightTime = new DateTime(daylightTime.Year, daylightTime.Month, daylightTime.Day, transitionTime.TimeOfDay.Hour, transitionTime.TimeOfDay.Minute, transitionTime.TimeOfDay.Second, DateTimeKind.Local); // Assuming local time

            return daylightTime;
        }

        // from https://stackoverflow.com/questions/6140018/how-to-calculate-2nd-friday-of-month-in-c-sharp
        private static DateTime NthOf(this DateTime currentDate, int occurrence, DayOfWeek day)
        {
            var firstDay = new DateTime(currentDate.Year, currentDate.Month, 1, 0, 0, 0, currentDate.Kind);

            var firstOccurrence = firstDay.DayOfWeek == day ? firstDay : firstDay.AddDays(day - firstDay.DayOfWeek);

            if (firstOccurrence.Month < currentDate.Month)
            {
                occurrence = occurrence + 1;
            }

            return firstOccurrence.AddDays(7 * (occurrence - 1));
        }

        private static DateTime? ConvertTimeByIanaTimeZoneId(DateTime? date, string fromIanaTimeZoneId, string toIanaTimeZoneId)
        {
            if (!date.HasValue)
            {
                return null;
            }

            var sourceTimeZone = FindTimeZoneInfo(IanaToWindows(fromIanaTimeZoneId));
            var destinationTimeZone = FindTimeZoneInfo(IanaToWindows(toIanaTimeZoneId));

            if (sourceTimeZone == null || destinationTimeZone == null)
            {
                throw new InvalidOperationException("Source or destination timezone not found.");
            }

            return TimeZoneInfo.ConvertTime(date.Value, sourceTimeZone, destinationTimeZone);
        }

        private static DateTime? ConvertTimeFromUtcByIanaTimeZoneId(DateTime? date, string toIanaTimeZoneId)
        {
            return ConvertTimeByIanaTimeZoneId(date, "Etc/UTC", toIanaTimeZoneId);
        }

        private static DateTime? ConvertTimeToUtcByIanaTimeZoneId(DateTime? date, string fromIanaTimeZoneId)
        {
            return ConvertTimeByIanaTimeZoneId(date, fromIanaTimeZoneId, "Etc/UTC");
        }

        private static TimeZoneInfo? FindTimeZoneInfo(string windowsOrIanaTimeZoneId)
        {
            return null;
        }

        private static List<string>? GetWindowsTimeZoneIds(bool ignoreTimeZoneNotFoundException = true)
        {
            return null;
        }

        private static void GetTimezoneMappings()
        {
            if (_windowsTimeZoneMappings.Count > 0 && _ianaTimeZoneMappings.Count > 0)
            {
                return;
            }

            lock (_syncObj)
            {
                if (_windowsTimeZoneMappings.Count > 0 && _ianaTimeZoneMappings.Count > 0)
                {
                    return;
                }

                var assembly = typeof(TimezoneHelper).GetAssembly();
                var resourceNames = assembly.GetManifestResourceNames();

                var resourceName = resourceNames.First(r => r.Contains("WindowsZones.xml"));

                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream != null)
                    {
                        byte[] bytes = stream.GetAllBytes();
                        string xmlString = Encoding.UTF8.GetString(bytes, 3, bytes.Length - 3); // Skipping byte order mark
                        XmlDocument xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(xmlString);
                        var windowsMappingNodes = xmlDocument.SelectNodes("//supplementalData/windowsZones/mapTimezones/mapZone[@territory='001']");
                        var ianaMappingNodes = xmlDocument.SelectNodes("//supplementalData/windowsZones/mapTimezones/mapZone");
                        if (windowsMappingNodes != null)
                        {
                            AddWindowsMappingsToDictionary(_windowsTimeZoneMappings, windowsMappingNodes);
                        }

                        if (ianaMappingNodes != null)
                        {
                            AddIanaMappingsToDictionary(_ianaTimeZoneMappings, ianaMappingNodes);
                        }
                    }
                }
            }
        }

        private static void AddWindowsMappingsToDictionary(Dictionary<string, string> timeZoneMappings, XmlNodeList defaultMappingNodes)
        {
            foreach (XmlNode defaultMappingNode in defaultMappingNodes)
            {
                var windowsTimezoneId = defaultMappingNode.GetAttributeValueOrNull("other");
                var ianaTimezoneId = defaultMappingNode.GetAttributeValueOrNull("type");
                if (windowsTimezoneId!.IsNullOrEmpty() || ianaTimezoneId!.IsNullOrEmpty())
                {
                    continue;
                }

                timeZoneMappings.Add(windowsTimezoneId!, ianaTimezoneId!);
            }
        }

        private static void AddIanaMappingsToDictionary(Dictionary<string, string> timeZoneMappings, XmlNodeList defaultMappingNodes)
        {
            foreach (XmlNode defaultMappingNode in defaultMappingNodes)
            {
                var ianaTimezoneId = defaultMappingNode.GetAttributeValueOrNull("type");
                var windowsTimezoneId = defaultMappingNode.GetAttributeValueOrNull("other");
                if (ianaTimezoneId!.IsNullOrEmpty() || windowsTimezoneId!.IsNullOrEmpty())
                {
                    continue;
                }

                ianaTimezoneId!
                    .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Where(id => !timeZoneMappings.ContainsKey(id))
                    .ToList()
                    .ForEach(ianaId => timeZoneMappings.Add(ianaId, windowsTimezoneId!));
            }
        }
    }
}