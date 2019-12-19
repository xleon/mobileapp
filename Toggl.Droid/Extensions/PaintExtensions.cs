using Android.Graphics;
using TogglPoint = Toggl.Shared.Point;

namespace Toggl.Droid.Extensions
{
    public static class PaintExtensions
    {
        public static void UpdatePaintForTextToFitWidth(this Paint paint, string text, float allowedWidth, Rect textSizeBounds)
        {
            // 48f is enough for reasonable resolution
            // (older phones could have issues with memory if this number is much larger)
            const float sampleTextSize = 48f;

            paint.GetTextBounds(text, 0, text.Length, textSizeBounds);

            if (textSizeBounds.Width() <= allowedWidth)
                return;

            paint.TextSize = sampleTextSize;
            paint.GetTextBounds(text, 0, text.Length, textSizeBounds);
            paint.TextSize = (int)(sampleTextSize * allowedWidth / textSizeBounds.Width());
        }
    }
}
