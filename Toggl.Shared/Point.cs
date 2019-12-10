namespace Toggl.Shared
{
    public struct Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Point Zero { get; } = new Point
        {
            X = 0,
            Y = 0,
        };
    }
}
