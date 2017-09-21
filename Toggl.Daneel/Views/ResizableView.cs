using System;
using System.Linq;
using Foundation;
using MvvmCross.Platform.Core;
using UIKit;
using static Toggl.Multivac.Extensions.ClampExtension;

namespace Toggl.Daneel.Views
{
    [Register(nameof(ResizableView))]
    public sealed class ResizableView : UIView
    {
        private readonly NSLayoutConstraint heightConstraint;
        private readonly UIPanGestureRecognizer panGestureRecognizer;
        private nfloat previousPointY;

        public event EventHandler HeightConstantChanged;

        public nfloat HeightConstant
        {
            get => heightConstraint.Constant;
            set
            {
                if (HeightConstant == value) return;
                heightConstraint.Constant = value.Clamp(MinHeight, MaxHeight);
                HeightConstantChanged.Raise(this);
            }
        }

        public nfloat MaxHeight { get; set; }

        public nfloat MinHeight { get; set; }

        public ResizableView(IntPtr handle) : base(handle)
        {
            heightConstraint = findOrCreateHeightConstraint();

            panGestureRecognizer = new UIPanGestureRecognizer(onDrag);
            AddGestureRecognizer(panGestureRecognizer );
        }

        private NSLayoutConstraint findOrCreateHeightConstraint()
        {
            var foundConstraint = Constraints.FirstOrDefault(c => c.FirstAttribute == NSLayoutAttribute.Height);

            if (foundConstraint != null)
                return foundConstraint;

            var createdConstraint = NSLayoutConstraint.Create(
                this,
                NSLayoutAttribute.Height,
                NSLayoutRelation.Equal,
                1,
                0
            );
            AddConstraint(createdConstraint);
            return createdConstraint;
        }

        private void onDrag()
        {
            var point = panGestureRecognizer.TranslationInView(this);
            var newHeight = HeightConstant + point.Y - previousPointY;

            HeightConstant = newHeight;

            previousPointY = panGestureRecognizer.State == UIGestureRecognizerState.Ended ? 0 : point.Y;
        }
    }
}
