using CoreAnimation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public class BarButtonEnabledTargetBinding : MvxTargetBinding<UIBarButtonItem, bool>
    {
        public const string BindingName = "Enabled";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public BarButtonEnabledTargetBinding(UIBarButtonItem target)
            : base(target)
        {
        }

        protected override void SetValue(bool value)
            => Target.Enabled = value;
    }
}
