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
