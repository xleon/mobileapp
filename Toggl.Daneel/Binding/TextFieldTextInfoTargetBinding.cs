using System;
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public class TextFieldTextInfoTargetBinding : MvxTargetBinding<UITextField, TextFieldInfo>
    {
        public const string BindingName = "TextFieldInfo";

        private readonly IDisposable selectedTextRangeDisposable;

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public TextFieldTextInfoTargetBinding(UITextField target)
            : base(target)
        {
            Target.EditingChanged += onEditingChanged;
            selectedTextRangeDisposable = Target.AddObserver(
                "selectedTextRange",
                NSKeyValueObservingOptions.New | NSKeyValueObservingOptions.Old,
                onSelectedTextRangeChanged
            );
        }

        protected override void SetValue(TextFieldInfo value)
        {
            Target.Text = value.Text;

            var beginnningOfDocument = Target.BeginningOfDocument;
            if (!Target.IsFirstResponder || beginnningOfDocument == null) return;

            var positionToSet = Target.GetPosition(beginnningOfDocument, value.CursorPosition);
            Target.SelectedTextRange = Target.GetTextRange(positionToSet, positionToSet);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing) return;
            selectedTextRangeDisposable?.Dispose();
            Target.EditingChanged -= onEditingChanged;
        }

        private void onEditingChanged(object sender, EventArgs e)
            => fireValueChanged();

        private void onSelectedTextRangeChanged(NSObservedChange change)
            => fireValueChanged();

        private void fireValueChanged()
        {
            var selectedRange = Target.SelectedTextRange?.Start;
            if (selectedRange == null) return;

            var cursorPosition = Target.GetOffsetFromPosition(Target.BeginningOfDocument, selectedRange);
            if (cursorPosition == null) return;

            var c = (int)cursorPosition;
            var textFieldInfo = new TextFieldInfo(Target.Text, c); 
            FireValueChanged(textFieldInfo);
        }
    }
}
