using System;

namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    public interface IMainLogKey : IEquatable<IMainLogKey>
    {
        long Identifier();
    }
}
