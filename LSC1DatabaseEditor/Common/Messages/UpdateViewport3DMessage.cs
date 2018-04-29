﻿using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.Common.Messages
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
