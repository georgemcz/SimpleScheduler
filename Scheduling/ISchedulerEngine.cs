namespace Simple.Scheduler
{
    /// <summary>
    /// Scheduler engine used for scheduling the tasks.
    /// </summary>
    public interface ISchedulerEngine
    {
        /// <summary>
        /// Starts the scheduling engine.
        /// </summary>
        void Start();

        /// <summary>
        /// Schedules the specified working item for execution.
        /// </summary>
        /// <param name="item">The working item.</param>
        void Schedule(IWorkingItem item);

        /// <summary>
        /// Schedules the specified configuration of working item for execution.
        /// </summary>
        /// <param name="configuration">The configuration of working item.</param>
        /// <returns>A scheduled working item.</returns>
        IWorkingItem Schedule(IWorkItemConfiguration configuration);

        /// <summary>
        /// Stops the scheduling engine.
        /// </summary>
        void Stop();
    }
}