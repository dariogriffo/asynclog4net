using System;
using log4net.Core;

namespace AsyncLog4net
{
    public class LoggingEventHelper
    {
        private static readonly Type HelperType = typeof(LoggingEventHelper);
        private readonly string _loggerName;

        public FixFlags Fix { get; set; }

        public LoggingEventHelper(string loggerName, FixFlags fix)
        {
            _loggerName = loggerName;
            Fix = fix;
        }

        public LoggingEvent CreateLoggingEvent(Level level, string message, Exception exception)
        {
            return new LoggingEvent(HelperType, null, _loggerName, level, message, exception) { Fix = Fix };
        }

    }
}
