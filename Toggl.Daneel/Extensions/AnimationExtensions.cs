using System;
using CoreAnimation;
using UIKit;
using static Toggl.Foundation.MvvmCross.Helper.Animation;

namespace Toggl.Daneel.Extensions
{
    public static class AnimationExtensions
    {
        public static CAMediaTimingFunction ToMediaTimingFunction(this CubicBezierCurve self)
            => new CAMediaTimingFunction(self.P0, self.P1, self.P2, self.P3);

        public static void Animate(double duration, CubicBezierCurve curve, Action changes, Action completion = null)
        {
            var timingFunction = curve.ToMediaTimingFunction();

            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = timingFunction;

            UIView.Animate(duration, 0.0f, UIViewAnimationOptions.TransitionNone, changes, completion);

            CATransaction.Commit();
        }
    }
}
