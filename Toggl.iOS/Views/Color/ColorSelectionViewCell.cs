using System;
using Foundation;
using Toggl.Core.UI.ViewModels;
using UIKit;
using Toggl.iOS.Extensions;
using Toggl.iOS.Cells;

namespace Toggl.iOS.Views
{
    public partial class ColorSelectionViewCell : BaseCollectionViewCell<SelectableColorViewModel>
    {
        public static readonly string Identifier = "colorSelectionViewCell";

        public static readonly NSString Key = new NSString(nameof(ColorSelectionViewCell));
        public static readonly UINib Nib;

        static ColorSelectionViewCell()
        {
            Nib = UINib.FromName(nameof(ColorSelectionViewCell), NSBundle.MainBundle);
        }

        protected ColorSelectionViewCell(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            SelectedImageView.Image = SelectedImageView.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            SelectedImageView.TintColor = UIColor.White;
        }

        protected override void UpdateView()
        {
            ColorCircleView.BackgroundColor = Item.Color.ToNativeColor();
            SelectedImageView.Hidden = !Item.Selected;
        }
    }
}
