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
            intent.Time_entry = new INObject(te.Id.ToString(), te.Description);
            intent.SuggestedInvocationPhrase = $"Suggested invocation phrase for {te.Description}";

            var response = StopTimerIntentResponse.SuccessIntentResponseWithTime_entry(intent.Time_entry);

            var interaction = new INInteraction(intent, response);
            interaction.Identifier = te.Id.ToString();
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