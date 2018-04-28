namespace LSC1DatabaseEditor.Common.Messages
{
    public class StartedTaskMessage
    {
        public string TaskName { get; set; }

        public StartedTaskMessage(string taskName)
        {;
            this.TaskName = taskName;
        }
    }
}
