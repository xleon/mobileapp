using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Toggl.Core.Extensions;
using Toggl.Core.Sync.States.Pull;
using Toggl.Networking;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using Toggl.Storage;

namespace Toggl.Core.Sync.States.PullTimeEntries
{
    internal sealed class FetchJustTimeEntriesSinceState : BaseFetchSinceState
    {
        public FetchJustTimeEntriesSinceState(
            ITogglApi api,
            ISinceParameterRepository since,
            ITimeService timeService)
            : base(api, since, timeService)
        {
        }

        protected override IFetchObservables Fetch()
        {
            var timeEntries =
                FetchRecentIfPossible(Api.TimeEntries.GetAllSince, FetchTwoMonthsOfTimeEntries)
                    .ConnectedReplay();

            var exception = new InvalidOperationException("Only Time entries were fetched.");

            return new FetchObservables(
                Observable.Throw<List<IWorkspace>>(exception),
                Observable.Throw<List<IWorkspaceFeatureCollection>>(exception),
                Observable.Throw<IUser>(exception),
                Observable.Throw<List<IClient>>(exception),
                Observable.Throw<List<IProject>>(exception),
                timeEntries,
                Observable.Throw<List<ITag>>(exception),
                Observable.Throw<List<ITask>>(exception),
                Observable.Throw<IPreferences>(exception));
        }
    }
}
