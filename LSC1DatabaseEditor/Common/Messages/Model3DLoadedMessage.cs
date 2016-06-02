using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
