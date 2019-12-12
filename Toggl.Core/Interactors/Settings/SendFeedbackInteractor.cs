using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Toggl.Core.DataSources.Interfaces;
using Toggl.Core.Models.Interfaces;
using Toggl.Networking.ApiClients;
using Toggl.Shared;
using Toggl.Storage;
using Toggl.Storage.Models;
using Toggl.Storage.Settings;

namespace Toggl.Core.Interactors.Settings
{
    public class SendFeedbackInteractor : IInteractor<IObservable<Unit>>
    {
        private readonly string message;
        private readonly ISingletonDataSource<IThreadSafeUser> userDataSource;
        private readonly IFeedbackApi feedbackApi;
        private readonly IInteractorFactory interactorFactory;

        public SendFeedbackInteractor(
            IFeedbackApi feedbackApi,
            ISingletonDataSource<IThreadSafeUser> userDataSource,
            IInteractorFactory interactorFactory,
            string message)
        {
            Ensure.Argument.IsNotNull(message, nameof(message));
            Ensure.Argument.IsNotNull(userDataSource, nameof(userDataSource));
            Ensure.Argument.IsNotNull(feedbackApi, nameof(feedbackApi));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));

            this.userDataSource = userDataSource;
            this.message = message;
            this.feedbackApi = feedbackApi;
            this.interactorFactory = interactorFactory;
        }

        public IObservable<Unit> Execute()
            => interactorFactory.GetFeedbackInfo().Execute()
                .SelectMany(data =>
                    userDataSource.Get().SelectMany(user =>
                        feedbackApi.Send(user.Email, message, data).ToObservable()));
    }
}
