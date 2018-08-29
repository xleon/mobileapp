using System;
using Foundation;
using Intents;
using Toggl.Daneel.Intents;
using Toggl.Foundation;
using Toggl.Foundation.Services;
using Toggl.Multivac.Models;

namespace Toggl.Daneel.Services
{
    public class IntentDonationService : IIntentDonationService
    {
        public void DonateStopCurrentTimeEntry()
        {
            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = "Stop timer";

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(onCompletion);
        }

        public void DonateShowReport(DonationReportPeriod period)
        {
            var intent = new ShowReportPeriodIntent();
            switch (period)
            {
                case DonationReportPeriod.Today:
                    intent.Period = ShowReportPeriodReportPeriod.Today;
                    break;
                case DonationReportPeriod.Yesterday:
                    intent.Period = ShowReportPeriodReportPeriod.Yesterday;
                    break;
                case DonationReportPeriod.LastWeek:
                    intent.Period = ShowReportPeriodReportPeriod.LastWeek;
                    break;
                case DonationReportPeriod.LastMonth:
                    intent.Period = ShowReportPeriodReportPeriod.LastMonth;
                    break;
                case DonationReportPeriod.ThisMonth:
                    intent.Period = ShowReportPeriodReportPeriod.ThisMonth;
                    break;
                case DonationReportPeriod.ThisWeek:
                    intent.Period = ShowReportPeriodReportPeriod.ThisWeek;
                    break;
                case DonationReportPeriod.ThisYear:
                    intent.Period = ShowReportPeriodReportPeriod.ThisYear;
                    break;
                case DonationReportPeriod.Unknown:
                    intent.Period = ShowReportPeriodReportPeriod.Unknown;
                    break;
            }

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(onCompletion);
        }

        public void DonateShowReport()
        {
            var intent = new ShowReportIntent();
            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(onCompletion);
        }

        private Action<NSError> onCompletion
        {
            get
            {
                return error =>
                {
                    if (!(error is null))
                    {
                        Console.WriteLine($"Interaction donation failed: {error}");
                    }
                    else
                    {
                        Console.WriteLine("Successfully donated interaction.");
                    }
                };
            }
        }
    }
}