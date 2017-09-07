using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Schedules the recurring configuration of the task.
    /// </summary>
    public interface IWorkItemRepeatConfiguration : IHideObjectMembers, IWorkItemConfiguration
    {
        /// <summary>
        /// Configures the task to be repeated in the specified recurring period.
        /// </summary>
        /// <param name="repeatingTimeout">The repeating timeout.</param>
        /// <returns>The configured working item.</returns>
        IRecurrentWorkItemConfiguration ThenEvery(TimeSpan repeatingTimeout);

        /// <summary>
        /// Configures the task to be repeated once, after the repeat timeout elapses.  
        /// </summary>
        /// <param name="repeatTimeout">The repeat timeout.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem ThenAfter(TimeSpan repeatTimeout);
    }
}