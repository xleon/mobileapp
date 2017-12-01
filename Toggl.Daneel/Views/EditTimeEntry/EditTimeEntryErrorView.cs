using Foundation;
using System;
using UIKit;
using ObjCRuntime;
using MvvmCross.Core.ViewModels;

namespace Toggl.Daneel
{
    public sealed partial class EditTimeEntryErrorView : UIView
    {
        public string Text
        {
            get => Label.Text;
            set => Label.Text = value;
        }

        public IMvxCommand CloseCommand { get; set; }

        public EditTimeEntryErrorView (IntPtr handle) : base (handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            CloseButton.TouchUpInside += onCloseButtonTap;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            CloseButton.TouchUpInside -= onCloseButtonTap;
        }

        public static EditTimeEntryErrorView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib(nameof(EditTimeEntryErrorView), null, null);
            return Runtime.GetNSObject<EditTimeEntryErrorView>(arr.ValueAt(0));
        }

        private void onCloseButtonTap(object sender, EventArgs e)
            => CloseCommand?.Execute();
    }
}
