using System;
using System.Collections.Generic;

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides a recalculation facilities for the set of working items.
    /// </summary>
    public interface IWorkingItemQueueProcessor
    {
        /// <summary>
        /// Puts the specified working items to the pending queue.
        /// </summary>
        /// <param name="workingItems">The working items.</param>
        void Schedule(IEnumerable<IWorkingItem> workingItems);

        /// <summary>
        /// Processes the pending work items.
        /// </summary>
        /// <param name="engine">The scheduler engine we're working under.</param>
        /// <returns>
        /// A minimal timespan that we should wait for next execution round (unless new item is scheduled).
        /// </returns>
        TimeSpan ProcessPendingWorkItems(ISchedulerEngine engine);

        /// <summary>
        /// Stops this item processor and deschedules (aborts) all items.
        /// </summary>
        void Stop();
    }
}