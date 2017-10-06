using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class BarButtonAnimatedEnabledTargetBinding : MvxTargetBinding<UIBarButtonItem, bool>
    {
        public const string BindingName = "AnimatedEnabled";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public BarButtonAnimatedEnabledTargetBinding(UIBarButtonItem target)
            : base(target)
        {
        }

        protected override void SetValue(bool value)
        {
            AnimationExtensions.Animate(
                Animation.Timings.EnterTiming,
                Animation.Curves.SharpCurve,
                () =>
                {
                    // This slight color change is needed for the fade animation to work.
                    // Animating only the Enabled property has no effect.
                    // Blame Apple.
                    Target.TintColor = UIColor.White.ColorWithAlpha(value ? 1.0f : 0.99f);
                    Target.Enabled = value;
                }
            );
        }
    }
}
