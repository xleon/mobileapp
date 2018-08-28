using System;
using Intents;
using Toggl.Daneel.Intents;
using Toggl.Foundation;
using Toggl.Foundation.Services;
using Toggl.Multivac.Models;

namespace Toggl.Daneel.Services
{
    public class IntentDonationService: IIntentDonationService
    {
        public void DonateStopCurrentTimeEntry()
        {
            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = "Stop timer.";

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(error =>
            {
                if (!(error is null))
                {
                    Console.WriteLine($"Interaction donation failed: {error}");
                }
                else
                {
                    Console.WriteLine("Successfully donated interaction.");
                }
            });
        }
    }
}