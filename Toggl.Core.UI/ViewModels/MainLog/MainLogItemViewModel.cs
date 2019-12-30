using System;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Interfaces;
using Toggl.Core.UI.ViewModels.MainLog.Identity;

namespace Toggl.Core.UI.ViewModels.MainLog
{
    public abstract class MainLogItemViewModel : IDiffable<IMainLogKey>, IDiffableByIdentifier<MainLogItemViewModel>
    {
        public IMainLogKey Identity { get; protected set; }

        public long Identifier => Identity.Identifier();

        public abstract bool Equals(MainLogItemViewModel other);
    }
}
