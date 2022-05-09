using System;
using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Fusee.Engine.Imp.Graphics.Desktop
{
    internal class RenderCanvasImpSyncContext : SynchronizationContext
    {
        private readonly ConcurrentQueue<Tuple<SendOrPostCallback, object?>> _allCallbacks = new();

        public override void Post(SendOrPostCallback d, object? state)
        {
            _allCallbacks.Enqueue(Tuple.Create(d, state));
        }

        internal void ExecutePendingPostAwaits()
        {
            while (!_allCallbacks.IsEmpty)
            {
                _ = _allCallbacks.TryDequeue(out var callback);

                if (callback != null)
                {
                    try
                    {
                        var d = callback.Item1;
                        var state = callback.Item2;
                        d(state);
                    }
                    catch (Exception exception)
                    {
                        ExceptionDispatchInfo.Capture(exception).Throw();
                    }
                }
            }

            // should always be empty at this point, but nevertheless
            _allCallbacks.Clear();
        }
    }
}