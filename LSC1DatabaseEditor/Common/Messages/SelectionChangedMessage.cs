using System.Collections.Generic;

namespace LSC1DatabaseEditor.Common.Messages
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
