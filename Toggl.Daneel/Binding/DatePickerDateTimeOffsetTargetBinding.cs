using System;    
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class DatePickerDateTimeOffsetTargetBinding : MvxTargetBinding<UIDatePicker, DateTimeOffset>
    {
        public const string BindingName = "DateTimeOffset";
        
        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        private static readonly DateTimeOffset referenceDate = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero);
        
        public DatePickerDateTimeOffsetTargetBinding(UIDatePicker target) : base(target)
        {
            Target.ValueChanged += onValueChanged;
        } 
        
        protected override void SetValue(DateTimeOffset value)
            => Target.Date = toNSDate(value);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);
            
            if (!isDisposing) return;
            Target.ValueChanged -= onValueChanged;
        }

        private NSDate toNSDate(DateTimeOffset dateTimeOffset)
            => NSDate.FromTimeIntervalSinceReferenceDate((dateTimeOffset - referenceDate).TotalSeconds);

        private DateTimeOffset toDateTimeOffset(NSDate nsDate)
            => referenceDate.AddSeconds(nsDate.SecondsSinceReferenceDate);

        private void onValueChanged(object sender, EventArgs e)
            => FireValueChanged(toDateTimeOffset(Target.Date));
    }
}
