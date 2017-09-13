using MvvmCross.Binding.Combiners;
using System;
using System.Linq;
using System.Collections.Generic;
using MvvmCross.Binding.Bindings.SourceSteps;
using System.Text;
using UIKit;
using Foundation;
using System.Globalization;
using MvvmCross.Platform.UI;
using CoreGraphics;
using MvvmCross.Plugins.Color;
using Toggl.Foundation.MvvmCross.Helper;
using MvvmCross.Plugins.Color.iOS;

namespace Toggl.Daneel.Combiners
{
    public sealed class ProjectTaskClientValueCombiner : MvxFormatValueCombiner
    {
        private const string projectDotImageResource = "icProjectDot";

        public override bool TryGetValue(IEnumerable<IMvxSourceStep> steps, out object value)
        {
            var stepList = steps.ToList();
            if (stepList.Count < 5)
                throw new ArgumentException($"{nameof(ProjectTaskClientValueCombiner)} needs at least 5 values to work - project, task, client, font's cap height and project color");

            var project = stepList[0].GetValue().ToString();
            var task = stepList[1].GetValue()?.ToString();
            var client = stepList[2].GetValue()?.ToString();
            var fontHeight = (double)stepList[3].GetValue();
            var projectColor = stepList[4].GetValue().ToString();

            var text = buildString(project, task, client);

            var image = UIImage.FromBundle(projectDotImageResource).ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            var dot = new NSTextAttachment { Image = image };
            //There neeeds to be a space before the dot, otherwise the colors don't work
            var attributedString = new NSMutableAttributedString(" ");
            attributedString.Append(NSAttributedString.FromAttachment(dot));
            attributedString.Append(new NSAttributedString(text));

            verticallyCenterProjectDot(dot, fontHeight);

            if (!string.IsNullOrEmpty(projectColor))
                setProjectDotColor(attributedString, projectColor);

            if (!string.IsNullOrEmpty(client))
                setClientTextColor(attributedString, client);

            value = attributedString;

            return true;
        }

        private string buildString(string project, string task, string client)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(project))
                builder.Append($" {project}");
            
            if (!string.IsNullOrEmpty(task))
                builder.Append($": {task}");
            
            if (!string.IsNullOrEmpty(client))
                builder.Append($" {client}");

            return builder.ToString();
        }

        private void verticallyCenterProjectDot(NSTextAttachment dot, double fontHeight)
        {
            var imageSize = dot.Image.Size;
            var y = (fontHeight - imageSize.Height) / 2;
            dot.Bounds = new CGRect(0, y, imageSize.Width, imageSize.Height);
        }

        private void setProjectDotColor(NSMutableAttributedString text, string hexColor)
        {
            var range = new NSRange(0, 1);
            var color = MvxColor.ParseHexString(hexColor).ToNativeColor();
            var attributes = new UIStringAttributes { ForegroundColor = color };
            text.AddAttributes(attributes, range);
        }

        private void setClientTextColor(NSMutableAttributedString text, string client)
        {
            var range = new NSRange(text.Length - client.Length, client.Length);
            var attributes = new UIStringAttributes { ForegroundColor = Color.EditTimeEntry.ClientText.ToNativeColor() };
            text.AddAttributes(attributes, range);
        }
    }
}
