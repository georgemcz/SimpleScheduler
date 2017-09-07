using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Represents the recurring working item task. 
    /// Recurring task schedules self for the repeating execution, based on the specified parameters.
    /// </summary>
    internal class RecurringWorkingItem : WorkingItem
    {
        /// <summary>
        /// Signals if the working item is recurring.
        /// </summary>
        private readonly bool _recurring;

        /// <summary>
        /// Repeat for the specific time.
        /// </summary>
        private TimeSpan _repeatFor;

        /// <summary>
        /// Repeating period.
        /// </summary>
        private TimeSpan _repeatPeriod;

        /// <summary>
        /// Repeat until specific point in time.
        /// </summary>
        private DateTime _repeatUntil;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecurringWorkingItem"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="name">The task name.</param>
        /// <param name="startAt">The start at.</param>
        /// <param name="startAfter">The start after.</param>
        /// <param name="repeatFor">The repeat for.</param>
        /// <param name="repeatPeriod">The repeat period.</param>
        /// <param name="repeatUntil">The repeat until.</param>
        /// <param name="recurring">if set to <c>true</c> [recurring].</param>
        public RecurringWorkingItem(
            Action action, 
            string name, 
            DateTime startAt,
            TimeSpan startAfter,
            TimeSpan repeatFor, 
            TimeSpan repeatPeriod,
            DateTime repeatUntil, 
            bool recurring)
                : base(action, name, startAt, startAfter)
        {
            _repeatFor = repeatFor;
            _repeatPeriod = repeatPeriod;
            _repeatUntil = repeatUntil;
            _recurring = recurring;
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{Name} (R)";
        }

        /// <summary>
        /// Executes the specified task immediatelly.
        /// </summary>
        /// <param name="engine">The scheduling engine.</param>
        protected override void ExecuteItem(ISchedulerEngine engine)
        {
            lock (InstanceLock)
            {
                // if some repeat-for was specified, calculate new until time
                if (_repeatFor != TimeSpan.MinValue)
                {
                    _repeatUntil = Time.UtcNow + _repeatFor;
                    _repeatFor = TimeSpan.MinValue;
                }
            }

            // execute the task itself
            base.ExecuteItem(engine);

            lock (InstanceLock)
            {
                var now = Time.UtcNow;

                // reschedule the task, when needed (and when the task was not aborted)
                if ((_repeatPeriod != TimeSpan.MinValue) && (ExecutionTime != DateTime.MinValue))
                {
                    // calculate the new execution time
                    while (ExecutionTime <= now)
                        ExecutionTime = CalculateNextExecutionTime(_repeatPeriod);

                    // when the task is not recurring, reset the repeat period
                    if (_recurring == false)
                        _repeatPeriod = TimeSpan.MinValue;

                    // when the until time is not specified or is less than now, reschedule
                    if (_repeatUntil == DateTime.MinValue || ExecutionTime <= _repeatUntil)
                        engine.Schedule(this); // reschedule self
                }
            }
        }

        /// <summary>
        /// Calculates the next execution time.
        /// </summary>
        /// <param name="repeatPeriod">The repeat period.</param>
        /// <returns>A time of the next execution</returns>
        protected virtual DateTime CalculateNextExecutionTime(TimeSpan repeatPeriod)
        {
            return ExecutionTime + repeatPeriod;
        }
    }
}