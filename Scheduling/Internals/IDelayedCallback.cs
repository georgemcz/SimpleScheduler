using System;

namespace Simple.Scheduler
{
    /// <summary>
    /// Provides a mechanism for delayed function call. Function is called when the object is signalized
    /// or when the timeout period elapses.
    /// </summary>
    public interface IDelayedCallback
    {
        /// <summary>
        /// Signalizes this instance to invoke the callback.
        /// </summary>
        void Signalize();

        /// <summary>
        /// Registers the <paramref name="callback"/> to be invoked when
        /// this object is signalled, or when timeout occurred.
        /// </summary>
        /// <param name="callback">The callback function.</param>
        /// <param name="timeout">The timeout defined for the first notification.</param>
        void WaitForSignal(Func<object, bool, TimeSpan> callback, TimeSpan timeout);

        /// <summary>
        /// Aborts the registered callback and stops waiting for the invocation.
        /// </summary>
        /// <remarks>When the operation is currently in progress, waits until is finished.</remarks>
        void Abort();
    }
}