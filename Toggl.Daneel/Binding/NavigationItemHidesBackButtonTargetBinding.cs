using System;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class NavigationItemHidesBackButtonTargetBinding : MvxTargetBinding<UINavigationItem, bool>
    {
        public const string BindingName = "HidesBackButton";

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public NavigationItemHidesBackButtonTargetBinding(UINavigationItem target)
            : base(target)
        {
        }

        protected override void SetValue(bool value)
        {
            Target.SetHidesBackButton(value, true);
        }
    }
}
