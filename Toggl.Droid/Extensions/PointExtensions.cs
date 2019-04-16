using Android.Graphics;
using TogglPoint = Toggl.Shared.Point;

namespace Toggl.Droid.Extensions
{
    public static class PointExtensions
    {
        public static PointF ToPointF(this TogglPoint point)
            => new PointF((float)point.X, (float)point.Y);

        public static TogglPoint ToPoint(this PointF point)
            => new TogglPoint { X = point.X, Y = point.Y };
    }
}
