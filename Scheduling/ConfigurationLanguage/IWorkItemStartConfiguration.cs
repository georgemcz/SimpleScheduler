using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Basic configuration of the scheduled task - when the task is being started.
    /// </summary>
    public interface IWorkItemStartConfiguration : IHideObjectMembers, IWorkItemConfiguration
    {
        /// <summary>
        /// Configures the task to be started at the specified time.
        /// </summary>
        /// <param name="when">When the workitem will be activated.</param>
        /// <returns>The configured working item.</returns>
        IWorkItemRepeatConfiguration At(DateTime when);

        /// <summary>
        /// Configures the task to be started after the specified time, when scheduled.
        /// </summary>
        /// <param name="after">The time period after the the workitem will be activated.</param>
        /// <returns>The configured working item.</returns>
        IWorkItemRepeatConfiguration After(TimeSpan after);

        /// <summary>
        /// Configures the task to repeat itself every time period.
        /// </summary>
        /// <param name="period">
        /// The period in which the workitem will be activated, possibly recurrently.
        /// </param>
        /// <returns>The configured working item.</returns>
        IRecurrentWorkItemConfiguration Every(TimeSpan period);
    }
}