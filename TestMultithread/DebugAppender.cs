using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net.Appender;
using log4net.Core;

namespace TestMultithread
{
    public class DebugAppender : MemoryAppender
    {
        Random rnd = new Random(Environment.TickCount);
        public int LoggedEventCount { get { return m_eventsList.Count; } }

        protected override void Append(LoggingEvent loggingEvent)
        {
            Thread.Sleep(rnd.Next(30,100));
            base.Append(loggingEvent);
        }
    }
}
