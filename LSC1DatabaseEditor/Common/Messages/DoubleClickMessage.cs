using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace LSC1DatabaseEditor.Messages
{
    public class DoubleClickMessage
    {
        public MouseButtonEventArgs E { get; set; }
        public object Sender { get; set; }

        public DoubleClickMessage(object sender, MouseButtonEventArgs e)
        {
            E = e;
            Sender = sender;
        }
    }
}
