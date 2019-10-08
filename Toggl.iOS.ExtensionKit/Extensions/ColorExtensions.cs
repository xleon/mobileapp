using CoreGraphics;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ExtensionKit.Extensions
{
    public static class ColorExtensions
    {
        public static UIColor ToNativeColor(this Color color)
            => UIColor.FromRGBA(color.Red, color.Green, color.Blue, color.Alpha);
    }
}
