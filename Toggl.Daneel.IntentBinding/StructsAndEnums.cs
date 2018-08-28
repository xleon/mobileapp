using System;
using ObjCRuntime;

namespace Toggl.Daneel.Intents
{
    [Native]
    public enum StopTimerIntentResponseCode : long
    {
        Unspecified = 0,
        Ready,
        ContinueInApp,
        InProgress,
        Success,
        Failure,
        FailureRequiringAppLaunch
    }
}
