namespace Simple.Scheduler
{
    /// <summary>
    /// The interface for all executor providers
    /// </summary>
    public interface IWorkingItemExecutor // rename to ExecutionContext?
    {
        /// <summary>
        /// Executes the specified working item on the scheduling engine.
        /// </summary>
        /// <param name="engine">The scheduling engine.</param>
        /// <param name="item">The scheduled item.</param>
        void Execute(ISchedulerEngine engine, IWorkingItem item);
    }
}