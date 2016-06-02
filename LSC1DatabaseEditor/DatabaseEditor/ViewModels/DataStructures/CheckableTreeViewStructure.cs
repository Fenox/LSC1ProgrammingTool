using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace LSC1DatabaseEditor.ViewModel.DataStructures
{
    public class TreeViewItem
    {
        public string Text { get; set; }

        public bool IsExpanded { get; set; } = true;

        public ObservableCollection<CheckableItemWithSub> SubItems { get; set; } = new ObservableCollection<CheckableItemWithSub>();
    }

    public class CheckableItemWithSub
    {
        public string Text { get; set; }
        public bool Checked { get; set; }
        public string NumSubs
        {
            get
            {
                return "[" + SubItems.Count + "]";
            }
        }

        public ObservableCollection<TextItem> SubItems { get; set; } = new ObservableCollection<TextItem>();
    }

    public class TextItem
    {
        public string Text { get; set; }
    }
}
