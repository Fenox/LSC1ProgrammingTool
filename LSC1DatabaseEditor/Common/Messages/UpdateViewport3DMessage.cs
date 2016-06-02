using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.Messages
{
    public class UpdateViewport3DMessage
    {
        public Visual3D Visual { get; set; }
        public ViewportUpdateOperation Op { get; set; }

        public UpdateViewport3DMessage(Visual3D visual, ViewportUpdateOperation op)
        {
            Op = op;
            Visual = visual;
        }
    }

    public enum ViewportUpdateOperation
    {
        Add,
        Remove,
        Clear
    }
}
