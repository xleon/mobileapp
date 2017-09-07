using System.Globalization;
using Foundation;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class TextExtensions
    {
        private static readonly NSParagraphStyle paragraphStyle;
        private static readonly MvxRGBValueConverter converter = new MvxRGBValueConverter();
        private static readonly UIColor strokeColor = Color.StartTimeEntry.ProjectTokenBorder.ToNativeColor();

        static TextExtensions()
        {
            paragraphStyle = new NSMutableParagraphStyle()
            {
                MinimumLineHeight = 24,
                MaximumLineHeight = 24
            };
        }

        public static string GetDescription(this UITextView self, TextFieldInfo info)
        {
            if (string.IsNullOrEmpty(info.ProjectName))
                return self.Text;

            return self.Text.Replace(info.PaddedProjectName(), "");
        }

        public static string PaddedProjectName(this TextFieldInfo self)
            => string.IsNullOrEmpty(self.ProjectName) ? "" : $" {self.ProjectName} ";

        public static NSAttributedString GetAttributedText(this TextFieldInfo self)
        {
            var projectName = self.PaddedProjectName();
            var fullText = $"{self.Text}{projectName}";
            var result = new NSMutableAttributedString(fullText);

            result.AddAttributes(new UIStringAttributes
            {
                BaselineOffset = 5,
                ParagraphStyle = paragraphStyle,
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Regular),
            }, new NSRange(0, self.Text.Length));

            if (string.IsNullOrEmpty(self.ProjectColor))
                return result;

            var hexColor = self.ProjectColor;
            var color = (UIColor)converter.Convert(hexColor, typeof(MvxColor), null, CultureInfo.CurrentCulture);
            result.AddAttributes(new UIStringAttributes
            {
                BaselineOffset = 7,
                ForegroundColor = color,
                StrokeColor = strokeColor,
                ParagraphStyle = paragraphStyle,
                Font = UIFont.SystemFontOfSize(12, UIFontWeight.Regular),
            }, new NSRange(self.Text.Length, projectName.Length));

            return result;
        }
    }
}
