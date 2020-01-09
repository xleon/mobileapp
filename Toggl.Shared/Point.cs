using System;
using static System.Math;

namespace Toggl.Shared
{
    public struct Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        private Lazy<double> magnitude;
        private Lazy<double> angle;

        public Point(double x, double y)
        {
            X = x;
            Y = y;

            magnitude = new Lazy<double>(() => Sqrt(x * x + y * y));
            angle = new Lazy<double>(() => Atan2(y, x).ToDegrees());
        }

        public static Point Zero { get; } = new Point
        {
            X = 0,
            Y = 0,
        };

        public double Magnitude
            => magnitude.Value;

        public double AngleInDegrees
            => angle.Value;

        public static Point operator +(Point a, Point b)
            => new Point(a.X + b.X, a.Y + b.Y);

        public static Point operator -(Point a, Point b)
            => new Point(a.X - b.X, a.Y - b.Y);

        public static Point operator -(Point point)
            => new Point(-point.X, -point.Y);
    }
}
