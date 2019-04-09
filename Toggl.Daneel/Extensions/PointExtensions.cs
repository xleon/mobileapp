using CoreGraphics;
using Toggl.Shared;

namespace Toggl.Daneel.Extensions
{
    public static class PointExtensions
    {
        public static CGPoint ToCGPoint(this Point point)
            => new CGPoint(point.X, point.Y);

        public static Point ToTogglPoint(this CGPoint point)
            => new Point { X = point.X, Y = point.Y };
    }
}
