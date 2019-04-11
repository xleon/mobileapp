using MvvmCross.UI;

namespace Toggl.Core.UI.Parameters
{
    public sealed class ColorParameters
    {
        public MvxColor Color { get; set; }

        public bool AllowCustomColors { get; set; }

        public static ColorParameters Create(MvxColor color, bool allowCustomColor) => new ColorParameters
        {
            Color = color,
            AllowCustomColors = allowCustomColor
        };
    }
}
