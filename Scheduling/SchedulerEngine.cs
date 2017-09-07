using System;
using System.ComponentModel.Composition;
using SimpleScheduler.Logging;

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides a facility for scheduling the work items to be executed in the (approx.) planned time.
    /// Normally you do not need to use this one directly, use <see cref="Scheduler.Schedule"/>
    /// </summary>
    [Export(typeof(ISchedulerEngine))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SchedulerEngine : ISchedulerEngine
    {
        /// <summary>
        /// Logging interface.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// The processing engine for recalculating the working item scheduling times.
        /// </summary>
        private readonly IWorkingItemQueueProcessor _queueProcessor;

        /// <summary>
        /// Used timeout granularity.
        /// </summary>
        private readonly ISchedulingTimerGranularity _timerGranularity;

        /// <summary>
        /// Locks the operation modifications.
        /// </summary>
        private readonly IDelayedCallback _delayedCallback;

        /// <summary>
        /// States the started state of scheduler.
        /// </summary>
        private bool _started;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulerEngine"/> class.
        /// </summary>
        /// <param name="delayedCallback">The delayed callback strategy.</param>
        /// <param name="queueProcessor">The scheduling processor.</param>
        /// <param name="timerGranularity">The timer granularity.</param>
        [ImportingConstructor]
        public SchedulerEngine(
            IDelayedCallback delayedCallback,
            IWorkingItemQueueProcessor queueProcessor, 
            ISchedulingTimerGranularity timerGranularity)
        {
            _log = LogProvider.GetLogger("Scheduler");
            _queueProcessor = queueProcessor;
            _timerGranularity = timerGranularity;
            _delayedCallback = delayedCallback;
        }

        /// <summary>
        /// Starts the scheduling engine.
        /// </summary>
        public void Start()
        {
            if (_started)
                throw new InvalidOperationException("The scheduler is already started!");

            _delayedCallback.WaitForSignal(FlushQueue, -1.ms());
            _started = true;
        }

        /// <summary>
        /// Schedules the specified working item for execution.
        /// </summary>
        /// <param name="item">The working item.</param>
        public void Schedule(IWorkingItem item)
        {
            if (!_started)
            {
                var message = $"Scheduler is not started (trying to schedule '{item}')";
                throw new InvalidOperationException(message);
            }

            DateTime plannedExecutionTime = item.ExecutionTime;

            // check that we're not trying to schedule a task that was already aborted
            if (plannedExecutionTime == DateTime.MinValue)
                throw new ArgumentException($"Cannot schedule aborted task! ({item})");

            ////_log.Trace(
            ////    "Scheduling the {0} to be fired at {1}",
            ////    item.ToString(),
            ////    plannedExecutionTime.ToString("dd.MM.yyyy HH:mm:ss.ffff"));

            // append the new item to the queue of pending working items
            _queueProcessor.Schedule(new[] { item });

            // signal "something is in the queue"
            _delayedCallback.Signalize();
        }

        /// <summary>
        /// Schedules the specified configuration of working item for execution.
        /// </summary>
        /// <param name="configuration">The configuration of working item.</param>
        /// <returns>A scheduled working item.</returns>
        public IWorkingItem Schedule(IWorkItemConfiguration configuration)
        {
            if (!_started)
                throw new InvalidOperationException("Scheduler is not started");

            var workItem = configuration.ToWorkingItem();
            Schedule(workItem);
            return workItem;
        }

        /// <summary>
        /// Stops the scheduling engine.
        /// </summary>
        public void Stop()
        {
            _log.Trace("Initiating scheduler shutdown (waiting until the last round completes, if already invoked)");
            _started = false;

            _queueProcessor.Stop();

            _delayedCallback.Abort();

            _log.Debug("Scheduler is down");
        }

        /// <summary>
        /// Flushes the scheduled workitems queue.
        /// </summary>
        /// <param name="state">The state objet.</param>
        /// <param name="timeouted">if set to <c>true</c> the timeout is signalled.</param>
        /// <returns>A timeout requested for the next round.</returns>
        private TimeSpan FlushQueue(object state, bool timeouted)
        {
            // do you job
            TimeSpan waitingTimeout = _queueProcessor.ProcessPendingWorkItems(this);
            return _started ? _timerGranularity.RecalculateTimeout(waitingTimeout) : TimeSpan.Zero;
        }
    }
}