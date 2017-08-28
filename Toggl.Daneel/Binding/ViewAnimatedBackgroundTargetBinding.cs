using System;
using CoreAnimation;
using CoreGraphics;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class ViewAnimatedBackgroundTargetBinding : MvxTargetBinding<UIView, UIColor>
    {
        public const string BindingName = "AnimatedBackground";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public ViewAnimatedBackgroundTargetBinding(UIView target)
            : base(target)
        {
        }

        protected override void SetValue(UIColor value)
        {
            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = Animation.Curves.SharpCurve.ToMediaTimingFunction();

            UIView.Animate(Animation.Timings.EnterTiming, () => Target.BackgroundColor = value);

            CATransaction.Commit();
        }
    }
}
