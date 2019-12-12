using System;
using System.Collections.Generic;
using System.Reactive;
using Toggl.Core.Interactors.Settings;

namespace Toggl.Core.Interactors
{
    public partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<Unit>> SendFeedback(string message)
            => new SendFeedbackInteractor(
                api.Feedback,
                dataSource.User,
                this,
                message);

        public IInteractor<IObservable<Dictionary<string, string>>> GetFeedbackInfo()
            => new GetFeedbackInfoInteractor(
                dataSource.User,
                dataSource.Workspaces,
                dataSource.TimeEntries,
                platformInfo,
                userPreferences,
                lastTimeUsageStorage,
                timeService,
                this);
    }
}
