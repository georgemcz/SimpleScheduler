using System;
using System.Diagnostics;
// ReSharper disable InconsistentNaming

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides a set of extension methods to allow easier configuration of scheduling times.
    /// </summary>
    [DebuggerStepThrough]
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of milliseconds.
        /// </summary>
        /// <param name="time">The number of milliseconds.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in milliseconds.</returns>
        public static TimeSpan ms(this int time) { return TimeSpan.FromMilliseconds(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of seconds.
        /// </summary>
        /// <param name="time">The number of seconds.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in seconds.</returns>
        public static TimeSpan sec(this int time) { return TimeSpan.FromSeconds(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of minutes.
        /// </summary>
        /// <param name="time">The number of minutes.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in minutes.</returns>
        public static TimeSpan min(this int time) { return TimeSpan.FromMinutes(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of hours.
        /// </summary>
        /// <param name="time">The number of hours.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in hours.</returns>
        public static TimeSpan hours(this int time) { return TimeSpan.FromHours(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of hours.
        /// </summary>
        /// <param name="time">The number of hours.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in hours.</returns>
        public static TimeSpan hour(this int time) { return hours(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="time">The number of days.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in days.</returns>
        public static TimeSpan days(this int time) { return TimeSpan.FromDays(time); }

        /// <summary>
        /// Returns a <seealso cref="TimeSpan"/> that represents a specified number of days.
        /// </summary>
        /// <param name="time">The number of days.</param>
        /// <returns><see cref="TimeSpan"/> representing the requested time in days.</returns>
        public static TimeSpan day(this int time) { return days(time); }
    }
}