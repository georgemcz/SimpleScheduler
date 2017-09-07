using System;
using System.ComponentModel.Composition.Hosting;

namespace Simple.Scheduler
{
    /// <summary>
    /// Default system scheduler
    /// </summary>
    public static class DefaultScheduler
    {
        /// <summary>
        /// The default scheduler singleton instance
        /// </summary>
        private static readonly Lazy<ISchedulerEngine> Default = new Lazy<ISchedulerEngine>(CreateDefaultScheduler);

        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>
        /// The current global scheduler.
        /// </value>
        public static ISchedulerEngine Current { get; set; } = Default.Value;

        /// <summary>
        /// Creates the scheduler.
        /// </summary>
        /// <returns>A new scheduler instance</returns>
        private static ISchedulerEngine CreateDefaultScheduler()
        {
            var catalog = new AssemblyCatalog(typeof(DefaultScheduler).Assembly);

            var container = new CompositionContainer(catalog, true);

            var scheduler = container.GetExportedValue<ISchedulerEngine>();
            scheduler.Start();

            return scheduler;
        }
    }
}