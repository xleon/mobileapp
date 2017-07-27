using CoreText;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public partial class StartTimeEntryViewController : MvxViewController<StartTimeEntryViewModel>
    {
        public StartTimeEntryViewController() 
            : base(nameof(StartTimeEntryViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareTextFields();

            UIKeyboard.Notifications.ObserveWillShow(keyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(keyboardWillHide);

            var bindingSet = this.CreateBindingSet<StartTimeEntryViewController, StartTimeEntryViewModel>();

            bindingSet.Bind(CloseButton).To(vm => vm.BackCommand);

            bindingSet.Apply();
        }

        private void keyboardWillShow(object sender, UIKeyboardEventArgs e)
            => BottomDistanceConstraint.Constant = e.FrameBegin.Height + 0;

        private void keyboardWillHide(object sender, UIKeyboardEventArgs e)
            => BottomDistanceConstraint.Constant = 0;

        private void prepareTextFields()
        {
            var stringAttributes = new CTStringAttributes(
                new UIStringAttributes { ForegroundColor = Color.StartTimeEntry.Placeholder.ToNativeColor() }.Dictionary
            );

            DescriptionTextField.TintColor = Color.StartTimeEntry.Cursor.ToNativeColor();
            DescriptionTextField.AttributedPlaceholder =
                new NSAttributedString(Resources.StartTimeEntryPlaceholder, stringAttributes);

            DescriptionTextField.BecomeFirstResponder();
        }
    }
}

