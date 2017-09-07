using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides an extension functions for the scheduling.
    /// </summary>
    public static class SchedulingHelperExtensions
    {
        /// <summary>
        /// The scheduler retrieval function
        /// </summary>
        public static Func<ISchedulerEngine> SchedulerRetrievalFunc = () => DefaultScheduler.Current;

        /// <summary>
        /// Starts the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>The started working item.</returns>
        public static IWorkingItem Start(this IWorkItemConfiguration timer)
        {
            var workingItem = timer.ToWorkingItem();
            return workingItem.Start();
        }

        /// <summary>
        /// Starts the specified timer.
        /// </summary>
        /// <param name="timer">The timer.</param>
        /// <returns>The started working item.</returns>
        public static IWorkingItem Start(this IWorkingItem timer)
        {
            var scheduler = SchedulerRetrievalFunc();
            if (scheduler == null)
                throw new InvalidOperationException("No scheduler in context - todo create some (shared) here");

            scheduler.Schedule(timer);
            return timer;
        }

        /// <summary>
        /// Checks whether the timer is not null, and when it is not, aborts it.
        /// </summary>
        /// <param name="timer">The timer.</param>
        public static void Abort(this IWorkingItem timer)
        {
            timer?.Cancel();
        }
    }
}
