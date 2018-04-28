using LSC1DatabaseLibrary.LSC1Simulation;

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
