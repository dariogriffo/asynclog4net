using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;

namespace TestMultithread
{
    class Program
    {
        static void Main(string[] args)
        {

            var q = log4net.Config.XmlConfigurator.Configure();
            ILog log = log4net.LogManager.GetLogger(typeof(Program));
            log.Info("--------------------- Start ------------------------------");
            var taskList = new List<Task>();
            var rnd = new Random(Environment.TickCount);

            for (int i = 0; i < 10; ++i)
            {
                var t = Task.Factory.StartNew(() =>
                {
                    var roof = rnd.Next(600, 1500);                    
                    log.Debug(string.Format("Thread {0} will log {1} events", Thread.CurrentThread.ManagedThreadId, roof));
                    for (int j = 0; j < roof; ++j)
                    {
                        log.Debug(string.Format("Thread {0} - Value {1}", Thread.CurrentThread.ManagedThreadId, j));
                    }
                    log.Info(string.Format("Logged {0} events for Thread {1}", roof, Thread.CurrentThread.ManagedThreadId));
                });
                taskList.Add(t);
            }

            log.Info("--------------------- All tasks created ****-----------------------------");
            while (taskList.Any(t => !t.IsCompleted))
            {
                Thread.Sleep(100);
            }
            log.Info("--------------------- All queues processed ------------------------------");            

        }
    }
}
