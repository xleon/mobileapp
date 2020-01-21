using AndroidX.Core.Graphics;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using AndroidColor = Android.Graphics.Color;

namespace Toggl.Droid.Extensions
{
    public static class ColorExtensions
    {
        public static AndroidColor ToNativeColor(this Color color)
            => new AndroidColor(color.Red, color.Green, color.Blue, color.Alpha);

        public static AndroidColor WithOpacity(this AndroidColor color, float opacity)
        {
            opacity = opacity.Clamp(0, 1);

            var intColor = color.ToArgb();
            var changedColor = ColorUtils.SetAlphaComponent(intColor, (int)(255 * opacity));
            return new AndroidColor(changedColor);
        }
    }
}
