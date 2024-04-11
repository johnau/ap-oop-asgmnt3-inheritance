namespace TaskManagerCore.Model
{
    /// <summary>
    /// Represents time intervals used in task scheduling.
    /// </summary>
    public enum TimeInterval
    {
        /// <summary>
        /// No time interval.
        /// </summary>
        None = 0,

        /// <summary>
        /// Time interval of one hour.
        /// </summary>
        Hourly = 1,

        /// <summary>
        /// Time interval of one day (24 hours).
        /// </summary>
        Daily = 24,

        /// <summary>
        /// Time interval of one week (24 hours * 7 days).
        /// </summary>
        Weekly = 24 * 7,

        /// <summary>
        /// Time interval of two weeks (24 hours * 7 days * 2).
        /// </summary>
        Fortnightly = 24 * 7 * 2,

        /// <summary>
        /// Time interval of one month (24 hours * 7 days * 30 days).
        /// </summary>
        Monthly = 24 * 7 * 30,

        /// <summary>
        /// Time interval of one year (24 hours * 7 days * 365 days).
        /// </summary>
        Yearly = 24 * 7 * 365,
    }
}
