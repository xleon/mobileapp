using CoreText;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Converters;
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

            prepareViews();

            UIKeyboard.Notifications.ObserveWillShow(keyboardWillShow);
            UIKeyboard.Notifications.ObserveWillHide(keyboardWillHide);

            var source = new StartTimeEntryTableViewSource(SuggestionsTableView);
            SuggestionsTableView.Source = source;

            var timeSpanConverter = new TimeSpanToDurationValueConverter();
            var buttonColorConverter = new BoolToConstantValueConverter<UIColor>(
                Color.StartTimeEntry.ActiveButton.ToNativeColor(),
                Color.StartTimeEntry.InactiveButton.ToNativeColor()
            );

            var bindingSet = this.CreateBindingSet<StartTimeEntryViewController, StartTimeEntryViewModel>();

            //TableView
            bindingSet.Bind(source).To(vm => vm.Suggestions);

            //Text
            bindingSet.Bind(TimeLabel).To(vm => vm.ElapsedTime).WithConversion(timeSpanConverter);
            bindingSet.Bind(DescriptionTextField).To(vm => vm.RawTimeEntryText);

            //Buttons
            bindingSet.Bind(BillableButton)
                      .For(v => v.TintColor)
                      .To(vm => vm.IsBillable)
                      .WithConversion(buttonColorConverter);

            //Commands
            bindingSet.Bind(DoneButton).To(vm => vm.DoneCommand);
            bindingSet.Bind(CloseButton).To(vm => vm.BackCommand);
            bindingSet.Bind(BillableButton).To(vm => vm.ToggleBillableCommand);

            bindingSet.Apply();
        }

        private void keyboardWillShow(object sender, UIKeyboardEventArgs e)
            => BottomDistanceConstraint.Constant = e.FrameBegin.Height + 0;

        private void keyboardWillHide(object sender, UIKeyboardEventArgs e)
            => BottomDistanceConstraint.Constant = 0;

        private void prepareViews()
        {
            //This is needed for the ImageView.TintColor bindings to work
            BillableButton.SetImage(
                BillableButton.ImageForState(UIControlState.Normal)
                              .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate), 
                UIControlState.Normal
            );

            TimeLabel.Font = TimeLabel.Font.GetMonospacedDigitFont();

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

