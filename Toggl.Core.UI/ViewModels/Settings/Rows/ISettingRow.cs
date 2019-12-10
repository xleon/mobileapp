using Toggl.Shared.Extensions;

namespace Toggl.Core.UI.ViewModels.Settings.Rows
{
    public interface ISettingRow
    {
        string Title { get; }
        ViewAction Action { get; }
    }
}
