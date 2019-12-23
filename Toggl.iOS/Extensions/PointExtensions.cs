using CoreGraphics;
using Toggl.Shared;

namespace Toggl.iOS.Extensions
{
    public static class PointExtensions
    {
        public static CGPoint ToCGPoint(this Point point)
            => new CGPoint(point.X, point.Y);

        public static Point ToSharedPoint(this CGPoint point)
            => new Point(point.X, point.Y);
    }
}
