namespace LSC1DatabaseEditor.Common.Messages
{
    public class Model3DLoadedMessage
    {
        public string Path { get; set; }

        public Model3DLoadedMessage(string path)
        {
            Path = path;
        }
    }
}
