using System;
using System.Threading;

namespace Simple.Scheduler
{
    /// <summary>
    /// The working item represents a scheduled item.
    /// </summary>
    internal class WorkingItem : IWorkingItem
    {
        /// <summary>
        /// Instance lock.
        /// </summary>
        private readonly object _instanceLock = new object();

        /// <summary>
        /// When the task is being started.
        /// </summary>
        private readonly DateTime _startAt;

        /// <summary>
        /// The time after the task is started.
        /// </summary>
        private readonly TimeSpan _startAfter;

        /// <summary>
        /// Task itself
        /// </summary>
        private readonly Action _task;

        /// <summary>
        /// The scheduled execution time.
        /// </summary>
        private DateTime _executionTime = DateTime.MaxValue; // never

        /// <summary>
        /// Counter of the suspendations.
        /// </summary>
        private int _suspendCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingItem"/> class.
        /// </summary>
        /// <param name="task">The task being scheduled.</param>
        /// <param name="name">The task name.</param>
        /// <param name="startAt">The start at time.</param>
        /// <param name="startAfter">The start after span.</param>
        public WorkingItem(Action task, string name, DateTime startAt, TimeSpan startAfter)
        {
            Name = name;
            _task = task;
            _startAt = startAt;
            _startAfter = startAfter;
        }

        /// <summary>
        /// Gets the working item name.
        /// </summary>
        /// <value>The working item name.</value>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the execution time that is being scheduled for this working item.
        /// </summary>
        /// <value>The execution time.</value>
        public DateTime ExecutionTime
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_executionTime == DateTime.MaxValue)
                    {
                        // calculate execution time from configuration
                        if (_startAfter != TimeSpan.MinValue)
                        {
                            if (_startAt == DateTime.MinValue)
                            {
                                // start after the specified time from now
                                _executionTime = Time.UtcNow + _startAfter;
                            }
                            else
                            {
                                // start after the specified time from start time
                                _executionTime = _startAt + _startAfter;
                            }
                        }
                        else
                            _executionTime = _startAt;
                    }

                    return _executionTime;
                }
            }

            protected set
            {
                _executionTime = value;
            }
        }

        /// <summary>
        /// Gets the instance common lock.
        /// </summary>
        public object InstanceLock => _instanceLock;

        /// <summary>
        /// Aborts the execution of the working item.
        /// </summary>
        public void Cancel()
        {
            lock (InstanceLock)
            {
                ExecutionTime = DateTime.MinValue;
            }
        }

        /// <summary>
        /// Executes the specified task immediatelly.
        /// </summary>
        /// <param name="engine">The scheduling engine.</param>
        public void Execute(ISchedulerEngine engine)
        {
            lock (InstanceLock)
            {
                // check if the task is being aborted
                if (ExecutionTime == DateTime.MinValue)
                    return; // simply return - this will cause the task to be removed from the scheduler
            }

            ExecuteItem(engine);
        }

        /// <summary>
        /// Resumes the execution of the working item previously suspended by the <seealso cref="Suspend"/>.
        /// </summary>
        /// <remarks>
        /// The suspended task continues in internal reschedulling. Therefore when you resume suspended
        /// repeating task, it can be activated as it would not been suspended.
        /// Another consequence is that if you suspends the single-shot task and it expires during the
        /// suspended period, it will never be executed.
        /// </remarks>
        public void Resume()
        {
#pragma warning disable 0420
            if (Interlocked.Decrement(ref _suspendCount) < 0)
                _suspendCount = 0; // set the minimum resume count on zero (this is not exactly threadsafe...)
#pragma warning restore 0420
        }

        /// <summary>
        /// Suspends the execution of the working item. It can be resumed by the <seealso cref="Resume"/>.
        /// </summary>
        /// <remarks>
        /// The suspended task continues in internal reschedulling. Therefore when you resume suspended
        /// repeating task, it can be activated as it would not been suspended.
        /// Another consequence is that if you suspends the single-shot task and it expires during the
        /// suspended period, it will never be executed.
        /// </remarks>
        public void Suspend()
        {
#pragma warning disable 0420
            Interlocked.Increment(ref _suspendCount);
#pragma warning restore 0420
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return Name ?? string.Empty;
        }

        /// <summary>
        /// Executes the item.
        /// </summary>
        /// <param name="engine">The engine.</param>
        protected virtual void ExecuteItem(ISchedulerEngine engine)
        {
            if (_suspendCount == 0)
                _task(); // simply run the configured task
        }
    }
}