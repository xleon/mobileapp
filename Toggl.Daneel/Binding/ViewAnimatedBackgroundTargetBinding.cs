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
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () => Target.BackgroundColor = value
            );
        }
    }
}
