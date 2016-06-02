using LSC1DatabaseLibrary.LSC1Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.LSC1ProgramSimulator.Messages
{
    public class MoveMachineHeadMessage
    {
        public LSC1Orientation Orientation { get; set; }

        public MoveMachineHeadMessage(LSC1Orientation orientation)
        {
            Orientation = orientation;
        }
    }
}
