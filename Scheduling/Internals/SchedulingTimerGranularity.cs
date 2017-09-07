using System;
using System.ComponentModel.Composition;
using SimpleScheduler.Logging;

namespace Simple.Scheduler
{
    /// <summary>
    /// Calculator for scheduling timer granularity.
    /// </summary>
    [Export(typeof(ISchedulingTimerGranularity))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class SchedulingTimerGranularity : ISchedulingTimerGranularity
    {
        /// <summary>
        /// Logging interface.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Defined timer granularity.
        /// </summary>
        private readonly TimeSpan _timerGranularity;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingTimerGranularity"/> class.
        /// </summary>
        [ImportingConstructor]
        public SchedulingTimerGranularity()
            : this(LogProvider.GetLogger("Scheduler"), 10.ms())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchedulingTimerGranularity"/> class.
        /// </summary>
        /// <param name="log">The logging engine.</param>
        /// <param name="timerGranularity">The timer granularity.</param>
        public SchedulingTimerGranularity(ILog log, TimeSpan timerGranularity)
        {
            _log = log;
            _timerGranularity = timerGranularity;
        }

        /// <summary>
        /// Recalculates the timeout by applying granularity rules.
        /// </summary>
        /// <param name="waitingTimeout">The waiting timeout.</param>
        /// <returns>A new waiting timeout.</returns>
        public TimeSpan RecalculateTimeout(TimeSpan waitingTimeout)
        {
            if ((long)waitingTimeout.TotalMilliseconds < -1L)
            {
                _log.Trace( $"Invalid timeout range - {(long)waitingTimeout.TotalMilliseconds} patching.");    

                waitingTimeout = _timerGranularity;
            }

            // set the minimal waiting time and it's not the infinite timeout
            if (waitingTimeout != -1.ms() && (waitingTimeout < _timerGranularity))
                waitingTimeout = _timerGranularity;

            // waiting for longer timeouts that 24 days fails (see case 5840)
            if (waitingTimeout > 12.hours())
            {
                // _log.Trace("Single waiting item cannot be longer than 12 hours, scheduling empty cycle.");
                waitingTimeout = 12.hours();
            }

            return waitingTimeout;
        }
    }
}