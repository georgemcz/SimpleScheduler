using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Represents the recurring working item task. 
    /// Recurring task schedules self for the repeating execution, based on the specified parameters.
    /// Recalculation of next time period is done in local time.
    /// </summary>
    /// <seealso cref="RecurringWorkingItem" />
    internal class LocalTimeRecurringWorkingItem : RecurringWorkingItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalTimeRecurringWorkingItem"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="name">The task name.</param>
        /// <param name="startAt">The start at.</param>
        /// <param name="startAfter">The start after.</param>
        /// <param name="repeatFor">The repeat for.</param>
        /// <param name="repeatPeriod">The repeat period.</param>
        /// <param name="repeatUntil">The repeat until.</param>
        /// <param name="recurring">if set to <c>true</c> [recurring].</param>
        public LocalTimeRecurringWorkingItem(
            Action action, 
            string name, 
            DateTime startAt, 
            TimeSpan startAfter, 
            TimeSpan repeatFor,
            TimeSpan repeatPeriod, 
            DateTime repeatUntil, 
            bool recurring) 
            : base(
                    action, 
                    name, 
                    Time.ToUniversalTime(startAt), 
                    startAfter,
                    repeatFor, 
                    repeatPeriod, 
                    Time.ToUniversalTime(repeatUntil),
                    recurring)
        {
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return $"{Name} (RL)";
        }

        /// <summary>
        /// Calculates the next execution time.
        /// </summary>
        /// <param name="repeatPeriod">The repeat period.</param>
        /// <returns>
        /// A time of the next execution
        /// </returns>
        protected override DateTime CalculateNextExecutionTime(TimeSpan repeatPeriod)
        {
            var previousExec = Time.ToLocalTime(ExecutionTime);
            var next = previousExec + repeatPeriod;

            return Time.ToUniversalTime(next);
        }
    }
}