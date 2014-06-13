using System;
using System.Threading;

namespace AsyncLog4net
{

    internal class Mutex
    {
        private int _count;
        
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        public class Guard : IDisposable
        {
            private readonly Mutex _mutex;

            public Guard(Mutex mutex)
            {
                _mutex = mutex;

                while (Interlocked.Increment(ref _mutex._count) > 1)
                {
                    Interlocked.Decrement(ref _mutex._count);
                    _mutex._event.WaitOne(10);
                }
            }

            public void Dispose()
            {
                Interlocked.Decrement(ref _mutex._count);
                _mutex._event.Set();
            }
        }

        public Guard Lock()
        {
            return new Guard(this);
        }

        
    }
}
