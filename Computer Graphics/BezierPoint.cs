using System.Drawing;

namespace Computer_Graphics
{
    public class BezierPoint
    {
        public Point point;
        public Point control1;
        public Point control2;

        public BezierPoint(Point point)
        {
            this.point = point;
            this.control1 = new Point(point.X - 10, point.Y - 10);
            this.control2 = new Point(point.X + 10, point.Y + 10);
        }
    }
}
