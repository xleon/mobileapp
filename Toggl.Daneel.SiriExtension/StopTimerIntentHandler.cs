using System;
using ObjCRuntime;
using Toggl.Daneel.Intents;

namespace SiriExtension
{
    public class StopTimerIntentHandler : StopTimerIntentHandling
    {
        public StopTimerIntentHandler()
        {
        }

        public override void HandleStopTimer(StopTimerIntent intent, Action<StopTimerIntentResponse> completion)
        {
            throw new NotImplementedException();
        }
    }
}
