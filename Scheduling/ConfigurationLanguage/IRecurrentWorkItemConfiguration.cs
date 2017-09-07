using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// The extended configuration of recurring tasks.
    /// </summary>
    public interface IRecurrentWorkItemConfiguration : IHideObjectMembers, IWorkItemConfiguration
    {
        /// <summary>
        /// Configures the task to be repeated in it's recurrence period until the specified time.
        /// </summary>
        /// <param name="time">The time until the workitem will be active.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem Until(DateTime time);

        /// <summary>
        /// Configures the task to be repeated in it's recurrence period for the specified time period.
        /// </summary>
        /// <param name="timePeriod">The time period for which the workitem will be active.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem For(TimeSpan timePeriod);

        /// <summary>
        /// Repeation is recalculated in local time
        /// </summary>
        /// <returns></returns>
        IWorkingItem InLocalTime();
    }
}