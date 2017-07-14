using System;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class SuggestionsEmptyViewCell : UITableViewCell
    {
        private static readonly Random Random = new Random();

        private const int MinTaskWidth = 74;
        private const int MaxTaskWidth = 84;
        private const int MinProjectWidth = 42;
        private const int MaxProjectWidth = 84;
        private const int MinDescriptionWidth = 74;
        private const int MaxDescriptionWidth = 110;

        private static readonly UIColor[] Colors =
        {
            UIColor.FromRGB(197f / 255f, 107f / 255f, 255f / 255f),
            UIColor.FromRGB(006f / 255f, 170f / 255f, 245f / 255f),
            UIColor.FromRGB(241f / 255f, 195f / 255f, 063f / 255f)
        };

        public static readonly NSString Key = new NSString(nameof(SuggestionsEmptyViewCell));
        public static readonly UINib Nib;

        static SuggestionsEmptyViewCell()
        {
            Nib = UINib.FromName(nameof(SuggestionsEmptyViewCell), NSBundle.MainBundle);
        }

        protected SuggestionsEmptyViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TaskWidth.Constant = Random.Next(MinTaskWidth, MaxTaskWidth);
            ProjectWidth.Constant = Random.Next(MinProjectWidth, MaxProjectWidth);
            ProjectView.BackgroundColor = Colors[Random.Next(0, Colors.Length)];
            DescriptionWidth.Constant = Random.Next(MinDescriptionWidth, MaxDescriptionWidth);

            SetNeedsLayout();
            SetNeedsUpdateConstraints();
        }
    }
}
    