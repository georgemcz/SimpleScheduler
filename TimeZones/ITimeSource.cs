using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// The time source.
    /// </summary>
    public interface ITimeSource
    {
        /// <summary>
        /// Gets the current time in Utc.
        /// </summary>
        /// <value>The current time in Utc.</value>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current time in the timezone.
        /// </summary>
        /// <value>The current time in current timezone.</value>
        DateTime Now { get; }

        /// <summary>
        /// Gets the time zone used to calculate time in this time source.
        /// </summary>
        /// <value>The time zone.</value>
        ITimeZone TimeZone { get; }
    }
}