using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.Messages
{
    public class SelectionChangedMessage
    {
        public List<object> SelectedObjects { get; set; }

        public SelectionChangedMessage(List<object> selectedObjects)
        {
            SelectedObjects = selectedObjects;
        }
    }
}
