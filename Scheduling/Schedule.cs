using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// The Schedule factory. Creates and configures the scheduled tasks.
    /// </summary>
    /// <example>
    /// Simple scheduling with immediate start:
    /// <code>
    /// var _task = Schedule.Action(DoSomething)
    ///                     .After(5.sec())
    ///                     .ThenEvery(10.sec())
    ///                     .For(2.hours())
    ///                     .Start();
    /// </code>
    /// Or you can delay the start:
    /// <code>
    /// var _task = Schedule.Action(DoSomething)
    ///                     .Every(1.hour());
    /// // ... do some other stuff
    /// _task.Start();
    /// </code> 
    /// When you need to stop the task being executed, you can simply abort it:
    /// <code>_task.Abort()</code>
    /// Since then the task is unusable and need to be scheduled again.
    /// </example>
    public class Schedule : IHideObjectMembers
    {
        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>Configured working item.</returns>
        public static IWorkItemStartConfiguration Action(Action action)
        {
            return Action(action, action.Method.Name);
        }
        
        /// <summary>
        /// Schedules the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="name">The action name.</param>
        /// <returns>Configured working item.</returns>
        public static IWorkItemStartConfiguration Action(Action action, string name)
        {
            // and schedule the action now
            return new WorkingItemConfigurator(action, name);
        }
    }
}