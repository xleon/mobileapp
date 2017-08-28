using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class TextFieldFocusTargetBinding : MvxTargetBinding<UITextField, bool>
    {
        public const string BindingName = "Focus";

        public override MvxBindingMode DefaultMode => MvxBindingMode.OneWay;

        public TextFieldFocusTargetBinding(UITextField target)
            : base(target)
        {
        }

        protected override void SetValue(bool value)
        {
            if (value)
            {
                Target.BecomeFirstResponder();
            }
            else
            {
                Target.ResignFirstResponder();
            }
        }
    }
}
