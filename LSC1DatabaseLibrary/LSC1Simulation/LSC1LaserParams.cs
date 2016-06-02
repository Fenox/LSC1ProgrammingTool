using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSC1DatabaseLibrary.LSC1Simulation
{
    public class LSC1LaserParams
    {
        public LSC1LaserParams() { }
        public LSC1LaserParams(LSC1LaserParams fromLaserParams)
        {
            BeamOn = fromLaserParams.BeamOn;
            Power = fromLaserParams.Power;
            C_Grip = fromLaserParams.C_Grip;
        }

        public bool BeamOn { get; set; }
        public int Power { get; set; }
        public int C_Grip { get; set; }
    }
}
