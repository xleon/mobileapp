using CoreGraphics;
using MvvmCross.iOS.Views;
using Toggl.Daneel.Presentation.Attributes;

namespace Toggl.Daneel.ViewControllers
{
    [ModalCardPresentation]
    public partial class EditTimeEntryViewController : MvxViewController
    {
        private const int switchHeight = 24;

        public EditTimeEntryViewController() : base(nameof(EditTimeEntryViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var scale = switchHeight / BillableSwitch.Frame.Height;
            BillableSwitch.Transform = CGAffineTransform.MakeScale(scale, scale);
        }
    }
}
