using System;
using Intents;
using Toggl.Daneel.Intents;
using Toggl.Foundation;
using Toggl.Multivac.Models;

namespace Toggl.Daneel.Services
{
    public class IntentDonationService: IIntentDonationService
    {
        public void StopTimeEntry(ITimeEntry te)
        {
            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = "Stop timer.";

            var response = StopTimerIntentResponse.SuccessIntentResponseWithEntry_description(te.Description, "Duration");

            var interaction = new INInteraction(intent, response);
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