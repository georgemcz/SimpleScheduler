using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Working item configurator.
    /// </summary>
    internal class WorkingItemConfigurator :
        IWorkItemStartConfiguration,
        IWorkItemRepeatConfiguration,
        IRecurrentWorkItemConfiguration
    {
        /// <summary>
        /// The planned task.
        /// </summary>
        private readonly Action _task;

        /// <summary>
        /// Configured point in time when the action should be executed.
        /// </summary>
        private DateTime _startAt = DateTime.MinValue;

        /// <summary>
        /// Configured period of time after the action should be executed.
        /// </summary>
        private TimeSpan _startAfter = TimeSpan.MinValue;

        /// <summary>
        /// Configured repeating period after which the task is repeated.
        /// </summary>
        private TimeSpan _repeatPeriod = TimeSpan.MinValue;

        /// <summary>
        /// Configured fact, defining if the task is recurring.
        /// </summary>
        private bool _recurring;

        /// <summary>
        /// Configured period of time for which the task is recurring.
        /// </summary>
        private TimeSpan _repeatFor = TimeSpan.MinValue;

        /// <summary>
        /// Repetitions are considered and recalculated in local time
        /// </summary>
        private bool _inLocalTime;

        /// <summary>
        /// Configured point in time until the task is recurring.
        /// </summary>
        private DateTime _repeatUntil = DateTime.MinValue;

        /// <summary>
        /// Signals that the configuration is already frozen.
        /// </summary>
        private bool _frozen;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkingItemConfigurator"/> class.
        /// </summary>
        /// <param name="task">The task action.</param>
        /// <param name="name">The task  name.</param>
        public WorkingItemConfigurator(Action task, string name)
        {
            Name = name;
            _task = task;
        }

        /// <summary>
        /// Gets or sets the work item name.
        /// </summary>
        /// <value>The work item name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Configures the task to be started at the specified time.
        /// </summary>
        /// <param name="when">When the workitem will be activated.</param>
        /// <returns>The configured working item.</returns>
        public IWorkItemRepeatConfiguration At(DateTime when)
        {
            RequireNotFrozen();

            _startAt = when;
            return this;
        }

        /// <summary>
        /// Configures the task to be started after the specified time, when scheduled.
        /// </summary>
        /// <param name="after">The time period after the the workitem will be activated.</param>
        /// <returns>The configured working item.</returns>
        public IWorkItemRepeatConfiguration After(TimeSpan after)
        {
            RequireNotFrozen();

            _startAfter = after;
            return this;
        }

        /// <summary>
        /// Configures the task to repeat itself every time period.
        /// </summary>
        /// <param name="period">The period in which the workitem will be activated, possibly recurrently.</param>
        /// <returns>The configured working item.</returns>
        public IRecurrentWorkItemConfiguration Every(TimeSpan period)
        {
            RequireNotFrozen();

            _startAfter = period; // start after the first period
            return ((IWorkItemRepeatConfiguration)this).ThenEvery(period); // and then continue
        }

        /// <summary>
        /// Configures the task to be repeated in the specified recurring period.
        /// </summary>
        /// <param name="repeatingTimeout">The repeating timeout.</param>
        /// <returns>The configured working item.</returns>
        IRecurrentWorkItemConfiguration IWorkItemRepeatConfiguration.ThenEvery(TimeSpan repeatingTimeout)
        {
            RequireNotFrozen();

            _repeatPeriod = repeatingTimeout;
            _recurring = true;
            return this;
        }

        /// <summary>
        /// Configures the task to be repeated once, after the repeat timeout elapses.
        /// </summary>
        /// <param name="repeatTimeout">The repeat timeout.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem IWorkItemRepeatConfiguration.ThenAfter(TimeSpan repeatTimeout)
        {
            RequireNotFrozen();

            _repeatPeriod = repeatTimeout;
            _recurring = false;
            return ToWorkingItem();
        }

        /// <summary>
        /// Configures the task to be repeated in it's recurrence period until the specified time.
        /// </summary>
        /// <param name="time">The time until the workitem will be active.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem IRecurrentWorkItemConfiguration.Until(DateTime time)
        {
            RequireNotFrozen();

            _repeatUntil = time;
            return ToWorkingItem();
        }

        /// <summary>
        /// Configures the task to be repeated in it's recurrence period for the specified time period.
        /// </summary>
        /// <param name="timePeriod">The time period for which the workitem will be active.</param>
        /// <returns>The configured working item.</returns>
        IWorkingItem IRecurrentWorkItemConfiguration.For(TimeSpan timePeriod)
        {
            RequireNotFrozen();

            _repeatFor = timePeriod; // -_repeatPeriod;
            return ToWorkingItem();
        }

        /// <summary>
        /// Repeation is recalculated in local time
        /// </summary>
        /// <returns></returns>
        public IWorkingItem InLocalTime()
        {
            RequireNotFrozen();
            _inLocalTime = true;
            return ToWorkingItem();
        }

        /// <summary>
        /// Converts the current configuration to the working item.
        /// </summary>
        /// <returns>A configured working item</returns>
        public IWorkingItem ToWorkingItem()
        {
            RequireNotFrozen();

            _frozen = true;

            if (_repeatPeriod != TimeSpan.MinValue)
            {
                if (_inLocalTime)
                {
                    // repeat period is configured, we will create the recurring work item
                    return new LocalTimeRecurringWorkingItem(
                                                        _task,
                                                        Name,
                                                        _startAt,
                                                        _startAfter,
                                                        _repeatFor,
                                                        _repeatPeriod,
                                                        _repeatUntil,
                                                        _recurring);
                }

                // repeat period is configured, we will create the recurring work item
                return new RecurringWorkingItem(
                                            _task,
                                            Name,
                                            _startAt,
                                            _startAfter,
                                            _repeatFor,
                                            _repeatPeriod,
                                            _repeatUntil,
                                            _recurring);
            }

            // standard single time executing work item
            return new WorkingItem(_task, Name, _startAt, _startAfter);
        }

        /// <summary>
        /// Checks the state of the frozen - if the config object is already frozen, throws.
        /// </summary>
        private void RequireNotFrozen()
        {
            if (_frozen)
                throw new InvalidOperationException("You cannot configure after ToWorkingItem call");
        }
    }
}