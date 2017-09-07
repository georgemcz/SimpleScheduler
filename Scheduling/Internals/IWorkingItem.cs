using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Represents a working item that is being scheduled for later execution.
    /// </summary>
    public interface IWorkingItem
    {
        /// <summary>
        /// Gets the execution time.
        /// </summary>
        /// <value>The execution time.</value>
        DateTime ExecutionTime { get; }

        /// <summary>
        /// Executes the specified task immediatelly.
        /// </summary>
        /// <param name="engine">The scheduling engine.</param>
        void Execute(ISchedulerEngine engine);

        /// <summary>
        /// Suspends the execution of the working item. It can be resumed by the <seealso cref="Resume"/>.
        /// </summary>
        void Suspend();

        /// <summary>
        /// Resumes the execution of the working item previously suspended by the <seealso cref="Suspend"/>.
        /// </summary>
        void Resume();

        /// <summary>
        /// Aborts the execution of the working item.
        /// </summary>
        void Cancel();
    }
}