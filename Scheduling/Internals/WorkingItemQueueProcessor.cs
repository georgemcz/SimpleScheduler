using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SimpleScheduler.Logging;

namespace Simple.Scheduler
{
    /// <summary>
    /// Processes the pending items and recalculates their execution time.
    /// </summary>
    [Export(typeof(IWorkingItemQueueProcessor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class WorkingItemQueueProcessor : IWorkingItemQueueProcessor
    {
        /// <summary>
        /// The working item executor.
        /// </summary>
        private readonly IWorkingItemExecutor _executor;

        /// <summary>
        /// Logging interface.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The appending lock
        /// </summary>
        private readonly object _appendLock = new object();

        /// <summary>
        /// An enumeration of work items.
        /// </summary>
        private IEnumerable<IWorkingItem> _workItems = new List<IWorkingItem>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingItemQueueProcessor"/> class.
        /// </summary>
        /// <param name="executor">The executing wrapper.</param>
        [ImportingConstructor]
        public WorkingItemQueueProcessor(IWorkingItemExecutor executor)
        {
            _log = LogProvider.GetLogger("Scheduler");
            _executor = executor;
        }

        /// <summary>
        /// Processes the pending work items.
        /// </summary>
        /// <param name="engine">The scheduler engine we're working under.</param>
        /// <returns>
        /// A minimal timespan that we should wait for next execution round (unless new item is scheduled).
        /// </returns>
        public TimeSpan ProcessPendingWorkItems(ISchedulerEngine engine)
        {
            ////_log.Trace("Processing scheduled items");

            // get the current available items, replace the pending items with empty list
            IEnumerable<IWorkingItem> items;
            lock (_appendLock)
            {
                items = _workItems;
                _workItems = new List<IWorkingItem>();
            }

            DateTime now = Time.UtcNow;

            List<IWorkingItem> pendingItems = new List<IWorkingItem>();
            DateTime closestItem = DateTime.MaxValue;
            TimeSpan nextExecution = -1.ms(); // define infinite timeout

            foreach (var item in items)
            {
                DateTime executionTime = item.ExecutionTime;

                if (executionTime == DateTime.MinValue)
                    continue; // this one was aborted, just ignore it

                if (executionTime <= now)
                {
                    ////_log.Trace(
                    ////    "Executing scheduled item {0}. Planned to: {1}",
                    ////    item.ToString(),
                    ////    executionTime.ToString("dd.MM.yyyy HH:mm:ss.ffff"));

                    _executor.Execute(engine, item);
                }
                else
                {
                    pendingItems.Add(item); // item is scheduled in future
                    closestItem = new DateTime(Math.Min(executionTime.Ticks, closestItem.Ticks));
                    nextExecution = closestItem - now;
                }
            }

            if (pendingItems.Count != 0)
            {
                Schedule(pendingItems); // re-schedule the still pending items
            }

            // recalculate the waiting timeout
            ////_log.Trace("Next round scheduled after {0}, (pending: {1})", nextExecution, pendingItems.Count);

            return nextExecution; // closestItem ought to be in future
        }

        /// <summary>
        /// Stops this item processor and deschedules (aborts) all items.
        /// </summary>
        public void Stop()
        {
            _log.Debug("Aborting scheduled items");

            lock (_appendLock)
            {
                foreach (var item in _workItems)
                {
                    if (item.ExecutionTime != DateTime.MinValue)
                    {
                        _log.Debug($"Aborting scheduled item {item}");
                        item.Abort();
                    }
                }

                _workItems = Enumerable.Empty<IWorkingItem>(); // no items set
            }
        }

        /// <summary>
        /// Puts the specified working items to the pending queue.
        /// </summary>
        /// <param name="workingItems">The working items.</param>
        public void Schedule(IEnumerable<IWorkingItem> workingItems)
        {
            lock (_appendLock)
            {
                // merge the new items with the already pending items
                var tempItems = _workItems.Concat(workingItems);
                _workItems = tempItems; // just to be sure, probably optimized out by compiler...
            }
        }
    }
}