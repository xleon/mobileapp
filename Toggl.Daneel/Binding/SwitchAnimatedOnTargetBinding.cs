using System;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class SwitchAnimatedOnTargetBinding : MvxTargetBinding<UISwitch, bool>
    {
        public const string BindingName = "AnimatedOn";

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public SwitchAnimatedOnTargetBinding(UISwitch target)
            : base(target)
        {
            Target.ValueChanged += onValueChanged;
        }

        protected override void SetValue(bool value)
        {
            Target.SetState(value, true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing)
                return;

            Target.ValueChanged -= onValueChanged;
        }

        private void onValueChanged(object sender, EventArgs e)
            => FireValueChanged(Target.On);
    }
}
