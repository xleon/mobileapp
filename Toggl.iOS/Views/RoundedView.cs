using CoreAnimation;
using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Toggl.iOS.Views
{
    [Register(nameof(RoundedView))]
    public sealed class RoundedView : UIView
    {
        private readonly CALayer maskingLayer = new CALayer();

        private bool roundLeft;
        public bool RoundLeft
        {
            get => roundLeft;
            set
            {
                if (roundLeft == value) return;
                roundLeft = value;
                updateMaskedCorners();
            }
        }

        private bool roungRight;
        public bool RoundRight
        {
            get => roungRight;
            set
            {
                if (roungRight == value) return;
                roungRight = value;
                updateMaskedCorners();
            }
        }

        private int cornerRadius;
        public int CornerRadius
        {
            get => cornerRadius;
            set
            {
                if (cornerRadius == value) return;
                cornerRadius = value;
                updateMaskedCorners();
            }
        }

        public override UIColor BackgroundColor
        {
            get => base.BackgroundColor;
            set
            {
                base.BackgroundColor = value;
                updateMaskedCorners();
            }
        }

        public RoundedView(IntPtr handle) : base(handle)
        {
        }

        public RoundedView(CGRect frame) : base(frame)
        {
            updateMaskedCorners();
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            updateMaskedCorners();
        }

        private void updateMaskedCorners()
        {
            Layer.CornerRadius = CornerRadius;
            Layer.MaskedCorners = 0;

            if (RoundLeft)
                Layer.MaskedCorners |= CACornerMask.MinXMaxYCorner | CACornerMask.MinXMinYCorner;

            if (RoundRight)
                Layer.MaskedCorners |= CACornerMask.MaxXMaxYCorner | CACornerMask.MaxXMinYCorner;
        }
    }
}
