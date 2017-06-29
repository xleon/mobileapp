using CoreAnimation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public class BarButtonAnimatedTintColorTargetBinding : MvxTargetBinding<UIBarButtonItem, UIColor>
    {
        public const string BindingName = "AnimatedTintColor";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public BarButtonAnimatedTintColorTargetBinding(UIBarButtonItem target)
            : base(target)
        {
        }

        protected override void SetValue(UIColor value)
        {
            CATransaction.Begin();
            CATransaction.AnimationTimingFunction = Animation.Curves.SharpCurve.ToMediaTimingFunction();

            UIView.Animate(Animation.Timings.EnterTiming, () => Target.TintColor = value);

            CATransaction.Commit();
        }
    }
}
