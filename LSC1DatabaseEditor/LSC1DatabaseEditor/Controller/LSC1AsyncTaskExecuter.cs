using GalaSoft.MvvmLight.Messaging;
using LSC1DatabaseEditor.Common.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC1DatabaseEditor.Controller
{
    public class LSC1AsyncTaskExecuter
    {
        public async Task<T> DoTaskAsync<T>(string taskName, Func<T> task)
        {
            Messenger.Default.Send(new StartedTaskMessage(taskName));
            var result = await Task.Run(task);
            Messenger.Default.Send(new EndedTaskMessage(taskName));
            return result;
        }

        public async Task DoTaskAsync(string taskName, Action task)
        {
            Messenger.Default.Send(new StartedTaskMessage(taskName));
            await Task.Run(task);
            Messenger.Default.Send(new EndedTaskMessage(taskName));
        }
    }
}
