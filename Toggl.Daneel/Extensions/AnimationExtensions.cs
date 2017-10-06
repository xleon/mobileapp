using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using static Toggl.Foundation.MvvmCross.Helper.Animation;

namespace Toggl.Daneel.Extensions
{
    public static class AnimationExtensions
    {
        public static CAMediaTimingFunction ToMediaTimingFunction(this CubicBezierCurve self)
            => new CAMediaTimingFunction(self.P0, self.P1, self.P2, self.P3);

        public static UICubicTimingParameters ToCubicTimingParameters(this CubicBezierCurve self)
            => new UICubicTimingParameters(
                new CGPoint(self.P0, self.P1),
                new CGPoint(self.P2, self.P3)
            );

        public static void Animate(double duration, CubicBezierCurve curve, Action changes, Action completion = null)
        {
            var buttonAnimator = new UIViewPropertyAnimator(duration, curve.ToCubicTimingParameters());
            buttonAnimator.AddAnimations(changes);

            if (completion != null)
                buttonAnimator.AddCompletion(_ => completion());
            
            buttonAnimator.StartAnimation();
        }
    }
}
