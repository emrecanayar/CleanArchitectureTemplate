namespace Core.Timing
{
    /// <summary>
    /// Defines interface for a DateTime range with timezone.
    /// </summary>
    public interface IZonedDateTimeRange : IDateTimeRange
    {
        /// <summary>
        /// Gets or sets the Timezone of the datetime range.
        /// </summary>
        string Timezone { get; set; }

        /// <summary>
        /// Gets or sets the StartTime with Offset.
        /// </summary>
        DateTimeOffset StartTimeOffset { get; set; }

        /// <summary>
        /// Gets or sets the EndTime with Offset.
        /// </summary>
        DateTimeOffset EndTimeOffset { get; set; }

        /// <summary>
        /// Gets or sets the StartTime in UTC.
        /// </summary>
        DateTime StartTimeUtc { get; set; }

        /// <summary>
        /// Gets or sets the EndTime in UTC.
        /// </summary>
        DateTime EndTimeUtc { get; set; }
    }
}
