using System.ComponentModel;

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides a configuration and conversion to working item.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IWorkItemConfiguration
    {
        /// <summary>
        /// Converts the current configuration to the working item.
        /// </summary>
        /// <returns>A configured working item</returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        IWorkingItem ToWorkingItem();
    }
}