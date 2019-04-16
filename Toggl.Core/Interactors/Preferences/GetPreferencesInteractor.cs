using System;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Interfaces;
using static Toggl.Shared.WorkspaceFeatureId;

namespace Toggl.Core.Interactors
{
    internal sealed class GetPreferencesInteractor : IInteractor<IObservable<IThreadSafePreferences>>
    {
        private readonly ITogglDataSource dataSource;

        public GetPreferencesInteractor(ITogglDataSource dataSource)
        {
            this.dataSource = dataSource;
        }

        public IObservable<IThreadSafePreferences> Execute() 
            => dataSource.Preferences.Current;
    }
}
