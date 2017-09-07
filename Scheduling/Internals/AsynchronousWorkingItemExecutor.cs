using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using SimpleScheduler.Logging;

namespace Simple.Scheduler
{
    /// <summary>
    /// Behavioral strategy, that executes each working item as a task in threadpool.
    /// </summary>
    [Export(typeof(IWorkingItemExecutor))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class AsynchronousWorkingItemExecutor : IWorkingItemExecutor
    {
        /// <summary>
        /// The warning threshold setting.
        /// </summary>
        private readonly int _warningThreshold;

        /// <summary>
        /// The logging engine.
        /// </summary>
        private readonly ILog _log;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousWorkingItemExecutor"/> class.
        /// </summary>
        [ImportingConstructor]
        public AsynchronousWorkingItemExecutor()
            : this(LogProvider.GetLogger("Scheduler"), 20)
        {  
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsynchronousWorkingItemExecutor"/> class.
        /// </summary>
        /// <param name="log">The logging engine.</param>
        /// <param name="warningThreshold">The warning threshold.</param>
        public AsynchronousWorkingItemExecutor(ILog log, int warningThreshold)
        {
            _log = log;
            _warningThreshold = warningThreshold;
        }

        /// <summary>
        /// Executes the specified working item on the scheduling engine.
        /// </summary>
        /// <param name="engine">The scheduling engine.</param>
        /// <param name="item">The scheduled item.</param>
        public void Execute(ISchedulerEngine engine, IWorkingItem item)
        {
            int workerThreads;
            int iocpThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out iocpThreads);

            if (workerThreads <= _warningThreshold)
                _log.Warn("Number of free threads in threadpool is low. Next async tasks will be only queued!");

            // new task based threading
            Task.Factory.StartNew(
                                  () =>
                                  {
                                      try
                                      {
                                          item.Execute(engine);
                                      }
                                      catch (Exception e)
                                      {
                                          // last resort exception handler
                                          _log.Error($"Unhandled exception in work item async processing {e.Message}");
                                      }
                                  },
                                  TaskCreationOptions.PreferFairness);
        }
    }
}