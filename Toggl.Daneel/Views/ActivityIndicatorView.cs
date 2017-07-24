using System;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Toggl.Daneel.Views
{
    [Register(nameof(ActivityIndicatorView))]
    public class ActivityIndicatorView : UIImageView
    {
        private readonly CABasicAnimation animation
            = CABasicAnimation.FromKeyPath("transform.rotation.z");

        private const string animationKey = "rotationAnimation";
        private const float animationDuration = 1;

        private string imageResource = "icLoader";
        public string ImageResource
        {
            get => imageResource;
            set => Image = UIImage.FromBundle(imageResource = value);
        }

        public ActivityIndicatorView(CGRect frame)
            : base (frame)
        {
            init();
        }

        public ActivityIndicatorView(IntPtr handle)
            : base(handle)
        {
        }

        private void init()
        {
            Image = UIImage.FromBundle(ImageResource);
            ContentMode = UIViewContentMode.Center;

            animation.Duration = animationDuration;
            animation.RepeatCount = float.PositiveInfinity;
            animation.By = NSObject.FromObject(Math.PI * 2);
            Layer.AddAnimation(animation, animationKey);
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            init();
        }
    }
}
