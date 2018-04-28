using System.Globalization;
using System.Windows.Media.Media3D;
using LSC1DatabaseEditor.LSC1DbEditor.ViewModels.DatabaseViewModel.NormalRows;

namespace LSC1DatabaseLibrary.LSC1Simulation
{
    public class LSC1Orientation
    {
        public LSC1Orientation() { }
        public LSC1Orientation(LSC1Orientation fromOrientation)
        {
            Position = new Point3D(fromOrientation.Position.X, fromOrientation.Position.Y, fromOrientation.Position.Z);
            Rotation = new Point3D(fromOrientation.Rotation.X, fromOrientation.Rotation.Y, fromOrientation.Rotation.Z);
        }

        public LSC1Orientation(DbPosRow pos)
        {
            Position.X = double.Parse(pos.X);
            Position.Y = double.Parse(pos.Y);
            Position.Z = double.Parse(pos.Z);
            Rotation.X = double.Parse(pos.RX);
            Rotation.Y = double.Parse(pos.RY);
            Rotation.Z = double.Parse(pos.RZ);
        }

        public LSC1Orientation(DbProcRobotRow procRobot)
        {
            Position.X = double.Parse(procRobot.X);
            Position.Y = double.Parse(procRobot.Y);
            Position.Z = double.Parse(procRobot.Z);
            Rotation.X = double.Parse(procRobot.RX);
            Rotation.Y = double.Parse(procRobot.RY);
            Rotation.Z = double.Parse(procRobot.RZ);
        }

        public Point3D Position;
        public Point3D Rotation;

        public void Transform(DbFrameRow frame)
        {
            Position.X += double.Parse(frame.X, CultureInfo.InvariantCulture);
            Position.Y += double.Parse(frame.Y, CultureInfo.InvariantCulture);
            Position.Z += double.Parse(frame.Z, CultureInfo.InvariantCulture);
            Rotation.X += double.Parse(frame.RX, CultureInfo.InvariantCulture);
            Rotation.Y += double.Parse(frame.RY, CultureInfo.InvariantCulture);
            Rotation.Z += double.Parse(frame.RZ, CultureInfo.InvariantCulture);
        }
    }
}
