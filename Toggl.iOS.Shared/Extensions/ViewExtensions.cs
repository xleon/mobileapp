using CoreGraphics;
using UIKit;

namespace Toggl.iOS.Shared.Extensions
{
    public static class ViewExtensions
    {
        public static void UpdateCardView(this UIView view, int cornerRadius = 8, int shadowRadius = 4, float shadowOpacity = 0.1f, CGSize? shadowOffset = null, UIColor shadowColor = null)
        {
            shadowOffset ??= new CGSize(0, 2);
            shadowColor ??= UIColor.Black;

            var shadowPath = UIBezierPath.FromRect(view.Bounds);
            view.Layer.ShadowPath?.Dispose();
            view.Layer.ShadowPath = shadowPath.CGPath;
            view.Layer.CornerRadius = cornerRadius;
            view.Layer.ShadowRadius = shadowRadius;
            view.Layer.ShadowOpacity = shadowOpacity;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowOffset = shadowOffset.Value;
            view.Layer.ShadowColor = shadowColor.CGColor;
        }
    }
}
