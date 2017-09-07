using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Represents the time in system. Use this class everywhere you need to access the system time.
    /// The <see cref="Time.Now"/> is automatically converted to the correct time zone.
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Initializes static members of the <see cref="Time"/> class.
        /// </summary>
        static Time()
        {
            DefaultTimeSource = new StandardTime();
        }

        /// <summary>
        /// Gets or sets the currently used time source.
        /// </summary>
        public static ITimeSource DefaultTimeSource { get; set; }

        /// <summary>
        /// Gets the current time in Utc.
        /// </summary>
        /// <value>The current time in Utc.</value>
        public static DateTime UtcNow => Current.UtcNow;

        /// <summary>
        /// Gets the current time in the timezone.
        /// </summary>
        /// <value>The current time in current timezone.</value>
        public static DateTime Now => Current.Now;

        /// <summary>
        /// Gets the current time source.
        /// </summary>
        /// <value>The current time source.</value>
        public static ITimeSource Current => DefaultTimeSource;

        /// <summary>
        /// Returns the coordinated universal time (UTC) that corresponds to a specified local time.
        /// </summary>
        /// <param name="time">The local date and time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the UTC time that corresponds to time.
        /// </returns>
        public static DateTime ToUniversalTime(DateTime time)
        {
            return Current.TimeZone.ToUniversalTime(time);
        }

        /// <summary>
        /// Returns the local time that corresponds to a specified coordinated universal time (UTC).
        /// </summary>
        /// <param name="time">A UTC time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the local time that corresponds to time.
        /// </returns>
        public static DateTime ToLocalTime(DateTime time)
        {
            return Current.TimeZone.ToLocalTime(time);
        }
    }
}