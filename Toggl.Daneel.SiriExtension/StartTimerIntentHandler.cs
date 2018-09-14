using System;
using SiriExtension.Models;
using Toggl.Daneel.ExtensionKit;
using Toggl.Daneel.Intents;
using Toggl.Ultrawave;

namespace SiriExtension
{
    public class StartTimerIntentHandler : StartTimerIntentHandling
    {
        private ITogglApi togglAPI;

        public StartTimerIntentHandler(ITogglApi togglAPI)
        {
            this.togglAPI = togglAPI;
        }

        public override void ConfirmStartTimer(StartTimerIntent intent, Action<StartTimerIntentResponse> completion)
        {
            if (togglAPI == null)
            {
                completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.FailureNoApiToken, null));
                return;
            }

            completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.Ready, null));        }

        public override void HandleStartTimer(StartTimerIntent intent, Action<StartTimerIntentResponse> completion)
        {
            var workspaceId = (long)Convert.ToDouble(intent.Workspace.Identifier);
            var timeEntry = new TimeEntry(workspaceId, null, null, false, DateTimeOffset.Now, null,
                                          intent.EntryDescription ?? "", new long[0], (long) SharedStorage.instance.GetUserId(), 0, null, DateTimeOffset.Now);
            togglAPI.TimeEntries.Create(timeEntry).Subscribe(te =>
            {
                SharedStorage.instance.SetNeedsSync(true);
                var response = new StartTimerIntentResponse(StartTimerIntentResponseCode.Success, null);
                completion(response);
            }, exception =>
            {
                completion(new StartTimerIntentResponse(StartTimerIntentResponseCode.Failure, null));
            });
        }
    }
}