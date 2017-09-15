using CoreGraphics;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalDialogPresentation]
    public partial class SelectDateTimeDialogViewController : MvxViewController<SelectDateTimeDialogViewModel>
    {
        public SelectDateTimeDialogViewController() : base(nameof(SelectDateTimeDialogViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareDatePicker();

            var bindingSet = this.CreateBindingSet<SelectDateTimeDialogViewController, SelectDateTimeDialogViewModel>();

            bindingSet.Bind(DatePicker)
                .For(v => v.BindDateTimeOffset())
                .To(vm => vm.DateTimeOffset);

            //Commands
            bindingSet.Bind(CloseButton).To(vm => vm.CloseCommand);
            bindingSet.Bind(SaveButton).To(vm => vm.SaveCommand);
            
            bindingSet.Apply();
        }

        private void prepareDatePicker()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                //ScreenWidth - 32 for 16pt margins on both sides
                Width = screenWidth > 320 ? screenWidth - 32 : 312,
                Height = View.Frame.Height
            };

            DatePicker.Locale = NSLocale.CurrentLocale;
        }
    }
}
