using System;
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;
using Toggl.Daneel.Extensions;

namespace Toggl.Daneel.Binding
{
    public sealed class DatePickerDateTimeOffsetTargetBinding : MvxTargetBinding<UIDatePicker, DateTimeOffset>
    {
        public const string BindingName = "DateTimeOffset";
        
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;
        
        public DatePickerDateTimeOffsetTargetBinding(UIDatePicker target) : base(target)
        {
            Target.ValueChanged += onValueChanged;
        }

        protected override void SetValue(DateTimeOffset value)
        {
            /* Related to issue https://github.com/toggl/mobileapp/issues/1887
             * We are setting the date to NSDate.Now here because of a glitch that causes the picker
             * to show the correct time, but an invalid date (Jan 1 or Dec 31).
             * This glitch seems to be related to the Xamarin wrapper, since I wasn't able to reproduce
             * the same problem in a Swift app.
             */
            Target.SetDate(NSDate.Now, false);
            Target.SetDate(value.ToNSDate(), true);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            
            if (!isDisposing) return;
            Target.ValueChanged -= onValueChanged;
        }

        private void onValueChanged(object sender, EventArgs e)
            => FireValueChanged(Target.Date.ToDateTimeOffset());
    }
}
