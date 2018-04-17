using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LSC1DatabaseEditor.Common.Messages
{
    public class EndedTaskMessage
    {
        public string TaskName { get; set; }

        public EndedTaskMessage(string taskName)
        {
            this.TaskName = taskName;                 
        }
    }
}
