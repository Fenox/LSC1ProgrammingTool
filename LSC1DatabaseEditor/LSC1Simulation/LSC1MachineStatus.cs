namespace LSC1DatabaseLibrary.LSC1Simulation
{
    public class LSC1MachineState
    {
        public LSC1MachineState() { }

        public LSC1MachineState(LSC1MachineState fromMachineStatus)
        {
            TCPOrientation = new LSC1Orientation(fromMachineStatus.TCPOrientation);
            LaserParameters = new LSC1LaserParams(fromMachineStatus.LaserParameters);
        }

        public LSC1Orientation TCPOrientation { get; set; } = new LSC1Orientation();
        public LSC1LaserParams LaserParameters { get; set; } = new LSC1LaserParams();
    }
}
