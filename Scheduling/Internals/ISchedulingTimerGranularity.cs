using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Defines the scheduling timer granularity.
    /// </summary>
    public interface ISchedulingTimerGranularity
    {
        /// <summary>
        /// Recalculates the timeout by applying granularity rules.
        /// </summary>
        /// <param name="waitingTimeout">The waiting timeout.</param>
        /// <returns>A new waiting timeout.</returns>
        TimeSpan RecalculateTimeout(TimeSpan waitingTimeout);
    }
}