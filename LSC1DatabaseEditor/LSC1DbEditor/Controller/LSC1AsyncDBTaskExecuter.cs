using System;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using NLog;

namespace LSC1DatabaseEditor.LSC1DbEditor.Controller
{
    public class LSC1AsyncDBTaskExecuter
    {
        private static readonly Mutex DbMutex = new Mutex(false, "DatabaseMutex");
        private static readonly Logger Logger = LogManager.GetLogger("Usage");


        public async Task<T> DoTaskAsync<T>(string taskName, Func<T> task)
        {
            Logger.Info("Added task " + taskName + "to task query");
            DbMutex.WaitOne();
            Messenger.Default.Send(new StartedTaskMessage(taskName));
            T result = await Task.Run(task);
            Messenger.Default.Send(new EndedTaskMessage(taskName));
            DbMutex.ReleaseMutex();
            return result;
        }
        
        public async Task<T> DoTaskAsync<T>(string taskName, Task<T> task)
        {
            Logger.Info("Added task " + taskName + "to task query");
            DbMutex.WaitOne();
            Messenger.Default.Send(new StartedTaskMessage(taskName));
            T result = await task;
            Messenger.Default.Send(new EndedTaskMessage(taskName));
            DbMutex.ReleaseMutex();
            return result;
        }

        public async Task DoTaskAsync(string taskName, Action task)
        {
            Logger.Info("Added task " + taskName + "to task query");
            DbMutex.WaitOne();
            Messenger.Default.Send(new StartedTaskMessage(taskName));
            await Task.Run(task);
            Messenger.Default.Send(new EndedTaskMessage(taskName));
            DbMutex.ReleaseMutex();
        }
    }
}
