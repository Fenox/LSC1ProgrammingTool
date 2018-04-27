using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;

namespace LSC1DatabaseEditor.LSC1CommonTool.Messages
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
