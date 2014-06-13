using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using NUnit.Framework;

namespace AsyncLog4net.Tests
{
    [TestFixture]
    public class AsyncAppenderShould
    {
        private ILog _syncLogger;
        private ILog _asyncLogger;
        private int _iterations;

        [SetUp]
        public void Setup()
        {
            var iterations = ConfigurationManager.AppSettings["iterations"];
            if (string.IsNullOrWhiteSpace(iterations))
            {
                iterations = "5000";
            }
            _iterations = int.Parse(iterations);
            log4net.Config.XmlConfigurator.Configure();
            _syncLogger = LogManager.GetLogger("syncLogger");
            _asyncLogger = LogManager.GetLogger("asyncLogger");
        }

        [Test]
        public void EndLoggingBeforeSyncAppender()
        {
            var now = Environment.TickCount;
            var syncTask = Task.Factory.StartNew(() => LogTask(_syncLogger, now));
            var asyncTask = Task.Factory.StartNew(() => LogTask(_asyncLogger, now));
            var tasks = new List<Task<int>>() {syncTask, asyncTask};
            while (!tasks.All(t=>t.IsCompleted))
            {
                Thread.Sleep(100);    
            }
            _syncLogger.Info(string.Format("sync:  {0}, async: {1}", syncTask.Result, asyncTask.Result));
            Console.WriteLine("sync:  {0}, async: {1}", syncTask.Result, asyncTask.Result);
            Assert.IsTrue(syncTask.Result > asyncTask.Result);

        }

        private int LogTask(ILog logger, int now)
        {
            var q = 0;
            for (var i = 0; i < _iterations; i++)
            {
                logger.Info(string.Format("async {0}", ++q));
            }
            return Environment.TickCount - now;
        }
    }

}
