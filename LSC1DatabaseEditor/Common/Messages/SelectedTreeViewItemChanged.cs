using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace LSC1DatabaseEditor.Messages.Common
{
    public class SelectedTreeViewItemChanged
    {
        public object Sender { get; set; }
        public object SelectedItem { get; set; }

        public SelectedTreeViewItemChanged(object sender, object selectedItem)
        {
            Sender = sender;
            SelectedItem = selectedItem;
        }
    }
}
