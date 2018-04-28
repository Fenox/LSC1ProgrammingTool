using System.Windows.Media.Media3D;

namespace LSC1DatabaseEditor.Messages
{
    public class CameraZoomMessage
    {
        public Rect3D Bounds { get; set; }
        public Point3D Center { get; set; }

        public CameraZoomMessage(Rect3D bounds, Point3D center)
        {
            Bounds = bounds;
            Center = center;
        }
    }
}
