using System;
using MvvmCross.Platform.Converters;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Multivac;
using UIKit;

namespace Toggl.Foundation.MvvmCross.Converters
{
    public class BoolToUIColorConverter : MvxValueConverter<bool, UIColor>
    {
        private readonly UIColor onColor;
        private readonly UIColor offColor;

        public BoolToUIColorConverter(MvxColor onColor, MvxColor offColor)
        {
            Ensure.Argument.IsNotNull(onColor, nameof(onColor));
            Ensure.Argument.IsNotNull(offColor, nameof(offColor));

            this.onColor = onColor.ToNativeColor();
            this.offColor = offColor.ToNativeColor();
        }

        protected override UIColor Convert(bool value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
            => value ? onColor : offColor;
    }
}
