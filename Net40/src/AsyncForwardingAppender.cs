using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Util;

namespace AsyncLog4net
{

    public static class ArrayEx
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> theEnumerable, Action<T> func)
        {
            if (theEnumerable == null) throw new ArgumentNullException("theEnumerable");
            if (func == null) throw new ArgumentNullException("func");
            foreach (var item in theEnumerable)
            {
                func(item);
            }
            return theEnumerable;
        }
    }


    public class AsyncForwardingAppender : ForwardingAppender
    {
        private FixFlags _fixFlags = FixFlags.Partial;
        private static readonly Type ThisType = typeof(AsyncForwardingAppender);
        private bool _shutDown;
        private readonly LoggingEventHelper _loggingEventHelper = new LoggingEventHelper("AsyncForwardingAppender", FixFlags.Partial);
        readonly Queue<LoggingEvent> _queue = new Queue<LoggingEvent>();
        readonly AutoResetEvent _event = new AutoResetEvent(false);
        private readonly Mutex _mutex = new Mutex();
        private bool _done;
        public bool WaitForAll { get; set; }

        public AsyncForwardingAppender()
        {
            var taskFactory = new TaskFactory(new OrderedTaskScheduler());
            taskFactory.StartNew(PollEvents);
        }

        private void PollEvents()
        {
            while (!_shutDown)
            {
                if (_queue.Any())
                {
                    var list = new LoggingEvent[_queue.Count()];
                    using (_mutex.Lock())
                    {
                        while (_queue.Any())
                        {
                            _queue.CopyTo(list, 0);
                            _queue.Clear();
                        }
                    }
                    if (!_shutDown || WaitForAll)
                    {
                        list.ForEach(ForwardLoggingEvent);
                    }
                }
                _event.WaitOne(50);
            }
            _done = true;
        }

        private void SetFixFlags(FixFlags newFixFlags)
        {
            if (newFixFlags != _fixFlags)
            {
                _loggingEventHelper.Fix = newFixFlags;
                _fixFlags = newFixFlags;
                InitializeAppenders();
            }
        }

        public FixFlags Fix
        {
            get { return _fixFlags; }
            set { SetFixFlags(value); }
        }

        public override void ActivateOptions()
        {
            base.ActivateOptions();
            InitializeAppenders();
        }

        protected override void OnClose()
        {
            _shutDown = true;
            _event.Set();
            while (!_done)
            {
                Thread.Sleep(20);
            }
            base.OnClose();
        }

        public override void AddAppender(IAppender newAppender)
        {
            base.AddAppender(newAppender);
            SetAppenderFixFlags(newAppender);
        }

        private void SetAppenderFixFlags(IAppender appender)
        {
            var bufferingAppender = appender as BufferingAppenderSkeleton;
            if (bufferingAppender != null)
            {
                bufferingAppender.Fix = Fix;
            }
        }

        private void InitializeAppenders()
        {
            foreach (var appender in Appenders)
            {
                SetAppenderFixFlags(appender);
            }
        }

        private void ForwardLoggingEvent(LoggingEvent loggingEvent)
        {
            try
            {
                base.Append(loggingEvent);
            }
            catch (Exception exception)
            {
                LogLog.Error(ThisType, "Unable to forward logging event", exception);
            }
        }

        private void AddEventToQueue(LoggingEvent loggingEvent)
        {
            loggingEvent.Fix = _fixFlags;
            _queue.Enqueue(loggingEvent);
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (_shutDown || loggingEvent == null) return;
            AddEventToQueue(loggingEvent);
            _event.Set();
        }

        protected override void Append(LoggingEvent[] loggingEvents)
        {
            if (_shutDown) return;
            using (_mutex.Lock())
            {
                foreach (var loggingEvent in loggingEvents.Where(e => e != null))
                {
                    AddEventToQueue(loggingEvent);
                }
            }
            _event.Set();
        }
    }
}
