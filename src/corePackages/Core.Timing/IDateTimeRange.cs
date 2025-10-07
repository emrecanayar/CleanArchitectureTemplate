namespace Core.Timing
{
    /// <summary>
    /// Defines interface for a DateTime range.
    /// </summary>
    public interface IDateTimeRange
    {
        /// <summary>
        /// Gets or sets start time of the datetime range.
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Gets or sets end time of the datetime range.
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// Gets or sets the time difference between the start and end times.
        /// </summary>
        TimeSpan TimeSpan { get; set; }
    }
}
