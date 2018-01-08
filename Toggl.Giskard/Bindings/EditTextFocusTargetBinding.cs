using System;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding;
using MvvmCross.Binding.Droid.Target;
using Toggl.Giskard.Extensions;

namespace Toggl.Giskard.Bindings
{
    public sealed class EditTextFocusTargetBinding : MvxAndroidTargetBinding<EditText, bool>
    {
        public const string BindingName = "Focus";

        public override MvxBindingMode DefaultMode => MvxBindingMode.Default;

        public EditTextFocusTargetBinding(EditText target)
                : base(target)
        {
        }

        protected override void SetValueImpl(EditText target, bool value)
        {
            target.RequestFocus();
        }
    }
}
