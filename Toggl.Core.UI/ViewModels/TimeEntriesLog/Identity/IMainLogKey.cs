using System;

namespace Toggl.Core.UI.ViewModels.TimeEntriesLog.Identity
{
    public interface IMainLogKey : IEquatable<IMainLogKey>
    {
        long Identifier();
    }
}
