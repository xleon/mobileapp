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

            value = buildAttributedString(project, task, client, projectColor, fontHeight);

            return true;
        }

        private static NSMutableAttributedString buildAttributedString(string project, string task, string client, 
                                                                       string projectColor, double fontHeight)
        {
            var dot = createDot(fontHeight, projectColor);
            var projectInfo = buildString(project, task, client);
            dot.Append(projectInfo);
            return dot;
        }

        private static NSMutableAttributedString createDot(double fontHeight, string hexColor)
        {
            var dot = new NSTextAttachment
            {
                Image = UIImage.FromBundle(projectDotImageResource)
                               .ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate)
            };

            var imageSize = dot.Image.Size;
            var y = (fontHeight - imageSize.Height) / 2;
            dot.Bounds = new CGRect(0, y, imageSize.Width, imageSize.Height);

            //There neeeds to be a space before the dot, otherwise the colors don't work
            var dotString = new NSMutableAttributedString(" ");
            var attachment = NSAttributedString.FromAttachment(dot);
            dotString.Append(attachment);

            return tryAddColorToDot(dotString, hexColor);
        }

        private static NSMutableAttributedString tryAddColorToDot(NSMutableAttributedString dotString, string hexColor)
        {
            if (string.IsNullOrEmpty(hexColor))
                return dotString;
            
            var range = new NSRange(0, 1);
            var color = MvxColor.ParseHexString(hexColor).ToNativeColor();
            var attributes = new UIStringAttributes { ForegroundColor = color };
            dotString.AddAttributes(attributes, range);

            return dotString;
        }

        private static NSAttributedString buildString(string project, string task, string client)
        {
            var builder = new StringBuilder();

            if (!string.IsNullOrEmpty(project))
                builder.Append($" {project}");
            
            if (!string.IsNullOrEmpty(task))
                builder.Append($": {task}");
            
            if (!string.IsNullOrEmpty(client))
                builder.Append($" {client}");
            
            return tryAddColorToClient(builder.ToString(), client);
        }

        private static NSAttributedString tryAddColorToClient(string text, string client)
        {
            var result = new NSMutableAttributedString(text);
            if (string.IsNullOrEmpty(client))
                return result;

            var range = new NSRange(text.Length - client.Length, client.Length);
            var attributes = new UIStringAttributes { ForegroundColor = Color.EditTimeEntry.ClientText.ToNativeColor() };
            result.AddAttributes(attributes, range);

            return result;
        }
    }
}