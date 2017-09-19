using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using MvvmCross.Binding;
using MvvmCross.Binding.Bindings.Target;
using MvvmCross.Platform.Core;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.Autocomplete;
using UIKit;

namespace Toggl.Daneel.Binding
{
    public sealed class TextViewTextInfoTargetBinding : MvxTargetBinding<UITextView, TextFieldInfo>
    {
        public const string BindingName = "TextFieldInfo";
        private const string selectedTextRangeChangedKey = "selectedTextRange";

        private TextFieldInfo textFieldInfo;

        private readonly IDisposable fireValueChangeDisposable;
        private readonly IDisposable selectedTextRangeDisposable;
        private readonly TextViewInfoDelegate infoDelegate = new TextViewInfoDelegate();
        private readonly Subject<TextFieldInfo> textFieldInfoSubject = new Subject<TextFieldInfo>();

        public override MvxBindingMode DefaultMode => MvxBindingMode.TwoWay;

        public TextViewTextInfoTargetBinding(UITextView target)
            : base(target)
        {
            Target.Delegate = infoDelegate;

            infoDelegate.TextChanged += onTextChanged;
            infoDelegate.ProjectDeleted += onProjectDeleted;

            selectedTextRangeDisposable = Target.AddObserver(
                selectedTextRangeChangedKey,
                NSKeyValueObservingOptions.OldNew,
                onSelectedTextRangeChanged
            );

            fireValueChangeDisposable = textFieldInfoSubject
                .AsObservable()
                .Subscribe(onTextFieldInfoChanged);
        }

        protected override void SetValue(TextFieldInfo value)
        {
            setTextFieldInfo(value);

            Target.AttributedText = value.GetAttributedText();

            if (!Target.IsFirstResponder || Target.BeginningOfDocument == null) return;

            var positionToSet = Target.GetPosition(Target.BeginningOfDocument, value.CursorPosition);
            Target.SelectedTextRange = Target.GetTextRange(positionToSet, positionToSet);
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing) return;

            Target.Delegate = null;

            fireValueChangeDisposable?.Dispose();
            selectedTextRangeDisposable?.Dispose();

            infoDelegate.TextChanged -= onTextChanged;
            infoDelegate.ProjectDeleted -= onProjectDeleted;
        }

        private void onTextChanged(object sender, EventArgs e)
            => queueValueChange();

        private void onSelectedTextRangeChanged(NSObservedChange change)
            => queueValueChange();

        private void onProjectDeleted(object sender, EventArgs e)
        {
            var description = Target.GetDescription(textFieldInfo);
            textFieldInfoSubject.OnNext(new TextFieldInfo(description, description.Length));
        }

        private void queueValueChange()
        {
            var selectedRangeStart = Target.SelectedTextRange?.Start;
            if (selectedRangeStart == null) return;

            var newDescription = Target.GetDescription(textFieldInfo);
            var descriptionLength = newDescription.Length;

            var newCursorPosition = (int)Target.GetOffsetFromPosition(Target.BeginningOfDocument, selectedRangeStart);
            var cursorIsInsideProjectToken = newCursorPosition > descriptionLength;
            if (!cursorIsInsideProjectToken)
            {
                textFieldInfoSubject.OnNext(textFieldInfo.WithTextAndCursor(newDescription, newCursorPosition));
                return;
            }

            var paddedProjectLength = textFieldInfo.PaddedProjectName().Length;
            var oldCursorPosition = textFieldInfo.CursorPosition;
            var isMovingForward = newCursorPosition >= oldCursorPosition;
            var actualCursorPosition = descriptionLength + (isMovingForward ? paddedProjectLength : 0);

            textFieldInfoSubject.OnNext(textFieldInfo.WithTextAndCursor(newDescription, actualCursorPosition));
        }

        private void onTextFieldInfoChanged(TextFieldInfo info)
        {
            setTextFieldInfo(info);
            FireValueChanged(info);
        }

        private void setTextFieldInfo(TextFieldInfo info)
        {
            if (textFieldInfo.Text != info.Text || textFieldInfo.ProjectName != info.ProjectName)
                Target.SetNeedsDisplay();

            textFieldInfo = info;
        }

        private sealed class TextViewInfoDelegate : NSObject, IUITextViewDelegate
        {
            public event EventHandler TextChanged;
            public event EventHandler ProjectDeleted;

            [Export("textViewDidChange:")]
            public void DidChange(UITextView textView)
                => TextChanged.Raise(this);
           
            [Export("textView:shouldChangeTextInRange:replacementText:")]
            public bool ShouldChangeCharacters(UITextView textView, NSRange range, string text)
            {
                if (!isPressingBackspace(range, text))
                    return true;

                var cursorPosition = range.Location;
                var attrs = textView.AttributedText.GetAttributes(cursorPosition, out var attrRange);
                var isDeletingInsideProjectNameTag = attrs.ObjectForKey(UIStringAttributeKey.ForegroundColor) != null;
                if (!isDeletingInsideProjectNameTag)
                    return true;

                textView.AttributedText = textView.AttributedText.Substring(0, attrRange.Location);
                ProjectDeleted.Raise(this);
                return false;
            }

            private static bool isPressingBackspace(NSRange range, string text)
                => range.Length == 1 && text.Length == 0;
        }
    }
}
