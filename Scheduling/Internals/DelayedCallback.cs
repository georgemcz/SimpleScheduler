using System;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Simple.Scheduler
{
    /// <summary>
    /// Calls the callback method when signalized or when the timeout interval elapsed.
    /// </summary>
    [ExcludeFromCodeCoverage]
    [Export(typeof(IDelayedCallback))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    internal class DelayedCallback : IDelayedCallback
    {
        /// <summary>
        /// Signalization event.
        /// </summary>
        private readonly AutoResetEvent _signal = new AutoResetEvent(false);

        /// <summary>
        /// Handle to the pending operation.
        /// </summary>
        private RegisteredWaitHandle _operationHandle;

        /// <summary>
        /// Actual callback function.
        /// </summary>
        private Func<object, bool, TimeSpan> _callback;

        /// <summary>
        /// Signalizes this instance to invoke the callback.
        /// </summary>
        public void Signalize()
        {
            _signal.Set();
        }

        /// <summary>
        /// Registers the <paramref name="callback"/> to be invoked when
        /// this object is signalled, or when timeout occurred.
        /// </summary>
        /// <param name="callback">The callback function.</param>
        /// <param name="timeout">The timeout defined for the first notification.</param>
        public void WaitForSignal(Func<object, bool, TimeSpan> callback, TimeSpan timeout)
        {
            _callback = callback;

            var handle = ThreadPool.RegisterWaitForSingleObject(
                _signal,
                InvokeCallback,
                null,
                timeout,
                true);

            // bit unnecessary as reference assignment is atomic, but just to avoid the r# warning here
            lock (_signal)
            {
                _operationHandle = handle;
            }
        }

        /// <summary>
        /// Aborts the registered callback and stops waiting for the invocation.
        /// </summary>
        /// <remarks>When the operation is currently in progress, waits until is finished.</remarks>
        public void Abort()
        {
            lock (_signal)
            {
                _operationHandle.Unregister(null);
            }
        }

        /// <summary>
        /// Invokes the callback and handles the callback re-registration.
        /// </summary>
        /// <param name="state">The state object.</param>
        /// <param name="timeouted">if set to <c>true</c> when the task timeouted.</param>
        private void InvokeCallback(object state, bool timeouted)
        {
            lock (_signal)
            {
                _operationHandle.Unregister(null);
                TimeSpan nextWait = _callback(state, timeouted);

                if (nextWait != TimeSpan.Zero)
                    WaitForSignal(_callback, nextWait);
            }
        }
    }
}