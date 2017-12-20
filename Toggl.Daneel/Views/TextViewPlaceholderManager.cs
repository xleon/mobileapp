using System;
using Foundation;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Views
{
    internal sealed class TextViewPlaceholderManager
    {
        private readonly UILabel label;
        private readonly UITextView textView;

        public TextViewPlaceholderManager(UITextView textView, UIColor color = null)
        {
            this.textView = textView;

            // AttributedText at this point has a length of 1, which is
            // a strange behaviour for an otherwise empty TextView.
            // It is set here to an empty string so that the initial visibility  
            // of the placeholder can be correctly inferred.
            textView.AttributedText = new NSAttributedString("");

            label = new UILabel()
            {
                Font = UIFont.SystemFontOfSize(textView.Font.PointSize),
                TextColor = color ?? Color.StartTimeEntry.Placeholder.ToNativeColor(),
                Text = Resources.StartTimeEntryPlaceholder,
                TextAlignment = UITextAlignment.Left,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            textView.AddSubview(label);
            setupConstraints();

            UpdateVisibility();
        }

        private void setupConstraints()
        {
            const float verticalPadding = 7;
            const float horizontalPadding = 5;

            label.CenterXAnchor.ConstraintEqualTo(textView.CenterXAnchor).Active = true;
            label.TopAnchor.ConstraintEqualTo(textView.TopAnchor, verticalPadding).Active = true;
            label.WidthAnchor.ConstraintEqualTo(textView.WidthAnchor, 1, -2 * horizontalPadding).Active = true;
            label.HeightAnchor.ConstraintEqualTo(2 * verticalPadding + textView.Font.PointSize).Active = true;
        }

        public void UpdateVisibility()
        {
            var hasText = textView.AttributedText?.Length > 0;
            var isInMultiStageEdit = textView.MarkedTextRange != null;

            label.Hidden = hasText || isInMultiStageEdit;
        }
    }
}
