using LSC1DatabaseLibrary.DatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.Messages
{
    public class LSC1JobChangedMessage
    {
        public DbJobNameRow NewJob { get; set; }

        public LSC1JobChangedMessage(DbJobNameRow newJob)
        {
            NewJob = newJob;
        }
    }
}
