//
// Copyright (c) Crossoverse Labs
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Crossoverse.Toolkit.DynamicResourceManager
{
    public class AsyncTaskEventArgs<TData> : EventArgs
    {
        public int Priority { get; }
        public TData Data { get; }
        
        public AsyncTaskEventArgs(int priority, TData data)
        {
            Priority = priority;
            Data = data;
        }
    }
    
    public class AsyncTaskDispatcher<TData> : IDisposable
    {
        /// <summary>
        /// Run on background thread
        /// </summary>
        public event Func<object, AsyncTaskEventArgs<TData>, CancellationToken, Task<bool>> AsyncTaskEvent;
        
        private readonly ConcurrentPriorityQueue<int, TData> _queue;
        private readonly CancellationTokenSource _ctsLoop;
        private readonly CancellationTokenSource _ctsAction;
        private readonly Thread _thread;
        private readonly int _frameDurationMilliseconds;
        
        public AsyncTaskDispatcher(int targetFrameRate = 30)
        {
            _frameDurationMilliseconds = (int)(1000.0f / targetFrameRate);
            
            _queue = new ConcurrentPriorityQueue<int, TData>();
            _ctsLoop = new CancellationTokenSource();
            _ctsAction = new CancellationTokenSource();
            
            _thread = new Thread(RunLoop);
            _thread.IsBackground = true;
            _thread.Start();
        }
        
        public void Dispose()
        {
            UnityEngine.Debug.Log($"[{nameof(AsyncTaskDispatcher<TData>)}] Disposing.");
            
            _ctsAction.Cancel();
            _ctsLoop.Cancel();
            _ctsAction.Dispose();
            _ctsLoop.Dispose();
            
            UnityEngine.Debug.Log($"[{nameof(AsyncTaskDispatcher<TData>)}] Disposed.");
        }
        
        public void Enqueue(int priority, TData data)
        {
            _queue.Enqueue(priority, data);
        }
        
        private async void RunLoop()
        {
            UnityEngine.Debug.Log($"[{nameof(AsyncTaskDispatcher<TData>)}] Beginning of RunLoop. Thread ID: {Environment.CurrentManagedThreadId}");
            
            var tic = new TimeSpan(DateTime.Now.Ticks);
            
            while (!_ctsLoop.IsCancellationRequested)
            {
                var toc = new TimeSpan(DateTime.Now.Ticks);
                if (toc.Subtract(tic).Duration().Milliseconds >= _frameDurationMilliseconds)
                {
                    tic = new TimeSpan(DateTime.Now.Ticks);
                    
                    if (_queue.Count > 0 && _queue.TryDequeue(out var keyValuePair))
                    {
                        if (AsyncTaskEvent != null)
                        {
                            var priority = keyValuePair.Key;
                            var data = keyValuePair.Value;
                            
                            var done = await AsyncTaskEvent.Invoke(this, new AsyncTaskEventArgs<TData>(priority, data), _ctsAction.Token);
                            
                            if (!done) { _queue.Enqueue(keyValuePair); }
                        }
                    }
                }
            }
            
            UnityEngine.Debug.Log($"[{nameof(AsyncTaskDispatcher<TData>)}] End of RunLoop.");
        }
    }
}