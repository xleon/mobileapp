using CoreAnimation;
using static Toggl.Foundation.MvvmCross.Helper.Animation;

namespace Toggl.Daneel.Extensions
{
    public static class AnimationExtensions
    {
        public static CAMediaTimingFunction ToMediaTimingFunction(this CubicBezierCurve self)
            => new CAMediaTimingFunction(self.P0, self.P1, self.P2, self.P3);
    }
}
