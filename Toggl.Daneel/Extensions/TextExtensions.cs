using System.Globalization;
using CoreGraphics;
using Foundation;
using MvvmCross.Platform.UI;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.Autocomplete;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static class TextExtensions
    {
        private static readonly NSParagraphStyle paragraphStyle;
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
            var color = MvxColor.ParseHexString(hexColor).ToNativeColor();
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

        public static NSAttributedString EndingWithTick(this string self, double fontHeight)
        {
            var tick = GetAttachmentString("icDoneSmall", fontHeight);

            var range = new NSRange(0, 1);
            var attributes = new UIStringAttributes { ForegroundColor = UIColor.White };
            tick.AddAttributes(attributes, range);

            var result = new NSMutableAttributedString(self);
            result.Append(tick);

            return result;
        }

        public static NSMutableAttributedString GetAttachmentString(this string imageName, double fontHeight)
        {
            var attachment = new NSTextAttachment
            {
                Image = UIImage.FromBundle(imageName)
                               .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)
            };

            var imageSize = attachment.Image.Size;
            var y = (fontHeight - imageSize.Height) / 2;
            attachment.Bounds = new CGRect(0, y, imageSize.Width, imageSize.Height);

            //There neeeds to be a space before the dot, otherwise the colors don't work
            var result = new NSMutableAttributedString(" ");
            var attachmentString = NSAttributedString.FromAttachment(attachment);
            result.Append(attachmentString);

            return result;
        }
    }
}
