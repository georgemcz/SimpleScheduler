using System;
using System.Linq;

namespace Simple.Scheduler
{
    /// <summary>
    /// Defines the time zone.
    /// </summary>
    public interface ITimeZone
    {
        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        /// <value>
        /// The time zone identifier.
        /// </value>
        string Id { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The timezone display name.</value>
        string DisplayName { get; }

        /// <summary>
        /// Gets the base UTC offset valid for this timezone.
        /// </summary>
        /// <value>
        /// The base UTC offset.
        /// </value>
        TimeSpan BaseUtcOffset { get; }

        /// <summary>
        /// Returns the local time that corresponds to a specified coordinated universal time (UTC).
        /// </summary>
        /// <param name="time">A UTC time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the local time that corresponds to time.
        /// </returns>
        DateTime ToLocalTime(DateTime time);

        /// <summary>
        /// Returns the coordinated universal time (UTC) that corresponds to a specified local time.
        /// </summary>
        /// <param name="time">The local date and time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the UTC time that corresponds to time.
        /// </returns>
        DateTime ToUniversalTime(DateTime time);

        /// <summary>
        /// Determines whether the specified time is invalid time in this timezone.
        /// </summary>
        /// <param name="time">A date and time value.</param>
        /// <returns>
        /// <c>true</c> if the specified time is invalid time; otherwise, <c>false</c>.
        /// </returns>
        bool IsInvalidTime(DateTime time);

        /// <summary>
        /// Determines whether the specified time is ambiguous time in this timezone.
        /// </summary>
        /// <param name="time">A date and time value.</param>
        /// <returns>
        /// <c>true</c> if the specified time is ambiguous time; otherwise, <c>false</c>.
        /// </returns>
        bool IsAmbiguousTime(DateTime time);
    }

    /// <summary>
    /// Represents the standard machine time source.
    /// </summary>
    public class StandardTime : ITimeSource
    {
        /// <summary>
        /// Gets the current time in Utc.
        /// </summary>
        /// <value>The current time in Utc.</value>
        public DateTime UtcNow => DateTime.UtcNow;

        /// <summary>
        /// Gets the current time in the timezone.
        /// </summary>
        /// <value>The current time in current timezone.</value>
        public DateTime Now => DateTime.Now;

        /// <summary>
        /// Gets the time zone used to calculate time in this time source.
        /// </summary>
        /// <value>The time zone.</value>
        public ITimeZone TimeZone => new TimeZone(TimeZoneInfo.Local);
    }

    /// <summary>
    /// Represents the TimeZone.
    /// </summary>
    public class TimeZone : ITimeZone
    {
        /// <summary>
        /// The currently used time zone information.
        /// </summary>
        private readonly TimeZoneInfo _timeZoneInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="TimeZone"/> class.
        /// </summary>
        /// <param name="tzi">The timezone information.</param>
        public TimeZone(TimeZoneInfo tzi)
        {
            _timeZoneInfo = tzi;
        }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        /// <value>
        /// The time zone identifier.
        /// </value>
        public string Id => _timeZoneInfo.Id;

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The timezone display name.</value>
        public string DisplayName => _timeZoneInfo.DisplayName;

        /// <summary>
        /// Gets the base UTC offset valid for this timezone.
        /// </summary>
        /// <value>
        /// The base UTC offset.
        /// </value>
        public TimeSpan BaseUtcOffset => _timeZoneInfo.BaseUtcOffset;

        /// <summary>
        /// Returns the local time that corresponds to a specified coordinated universal time (UTC).
        /// </summary>
        /// <param name="time">A UTC time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the local time that corresponds to time.
        /// </returns>
        public DateTime ToLocalTime(DateTime time)
        {
            return time.Kind == DateTimeKind.Local
                ? time
                : TimeZoneInfo.ConvertTimeFromUtc(time, _timeZoneInfo);
        }

        /// <summary>
        /// Returns the coordinated universal time (UTC) that corresponds to a specified local time.
        /// </summary>
        /// <param name="time">The local date and time.</param>
        /// <returns>
        /// A <see cref="T:System.DateTime"></see> instance whose value is the UTC time that corresponds to time.
        /// </returns>
        public DateTime ToUniversalTime(DateTime time)
        {
            if (time.Kind == DateTimeKind.Local && !_timeZoneInfo.Equals(TimeZoneInfo.Local))
            {
                // if the time is local, and we can only recalculate from local
                throw new ArgumentException(
                    "When specified the Local time (DateTime.Kind = Local), I can convert only from Local timezone!");
            }

            return time.Kind == DateTimeKind.Utc
                       ? time
                       : ConvertTimeToUtc(time);
        }

        /// <summary>
        /// Determines whether the specified time is invalid time in this timezone.
        /// </summary>
        /// <param name="time">A date and time value.</param>
        /// <returns>
        /// <c>true</c> if the specified time is invalid time; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInvalidTime(DateTime time)
        {
            return _timeZoneInfo.IsInvalidTime(time);
        }

        /// <summary>
        /// Determines whether the specified time is ambiguous time in this timezone.
        /// </summary>
        /// <param name="time">A date and time value.</param>
        /// <returns>
        /// <c>true</c> if the specified time is ambiguous time; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAmbiguousTime(DateTime time)
        {
            return _timeZoneInfo.IsAmbiguousTime(time);
        }

        /// <summary>
        /// Converts the time to UTC with checks and corrections for InvalidTime.
        /// </summary>
        /// <param name="time">The non-utc time.</param>
        /// <returns>A time converted to UTC</returns>
        private DateTime ConvertTimeToUtc(DateTime time)
        {
            // convert the time to Unspecified to avoid Local specific recalculations
            var calculatedTime = new DateTime(time.Ticks, DateTimeKind.Unspecified);

            if (_timeZoneInfo.IsInvalidTime(calculatedTime))
            {
                // it is invalid time due to DLT
                // I will blatantly assume, that the problem is the same for all TZs
                // as it is in our TZ
                var date = calculatedTime.Date;
                var adj = _timeZoneInfo.GetAdjustmentRules()
                            .FirstOrDefault(ru => ru.DateStart <= date && ru.DateEnd >= date);

                if (adj != null)
                {
                    var delta = adj.DaylightDelta;
                    calculatedTime = calculatedTime.Add(delta);
                }
            }

            return TimeZoneInfo.ConvertTimeToUtc(calculatedTime, _timeZoneInfo);
        }
    }
}