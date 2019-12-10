using System;
using System.Threading.Tasks;
using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Settings.Rows
{
    public class AnnotationRow : ISettingRow
    {
        public string Title { get; }
        public ViewAction Action { get; }

        public AnnotationRow(string text)
        {
            Title = text;
        }
    }
}
