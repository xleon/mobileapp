using System;
using Toggl.Ultrawave;
using Toggl.Daneel.Intents;
using Foundation;
using Intents;

namespace SiriExtension
{
    [Register("IntentHandler")]
    public class IntentHandler : INExtension
    {        
        protected IntentHandler(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override NSObject GetHandler(INIntent intent)
        {
            if (intent is StopTimerIntent)
            {
                return new StopTimerIntentHandler();
            }
            throw new Exception("Unhandled intent type: ${intent}");
        }
    }
}
