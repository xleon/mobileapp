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
        public void DonateStartTimeEntry()
        {
            var intent = new StartTimerIntent();
            intent.SuggestedInvocationPhrase = "Start timer";

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(onCompletion);
        }

        public void DonateStopCurrentTimeEntry()
        {
            var intent = new StopTimerIntent();
            intent.SuggestedInvocationPhrase = "Stop timer";

            var interaction = new INInteraction(intent, null);
            interaction.DonateInteraction(onCompletion);
        }

        public void DonateShowReport(ReportPeriod period)
        {
            var intent = new ShowReportPeriodIntent();
            switch (period)
            {
                case ReportPeriod.Today:
                    intent.Period = ShowReportPeriodReportPeriod.Today;
                    break;
                case ReportPeriod.Yesterday:
                    intent.Period = ShowReportPeriodReportPeriod.Yesterday;
                    break;
                case ReportPeriod.LastWeek:
                    intent.Period = ShowReportPeriodReportPeriod.LastWeek;
                    break;
                case ReportPeriod.LastMonth:
                    intent.Period = ShowReportPeriodReportPeriod.LastMonth;
                    break;
                case ReportPeriod.ThisMonth:
                    intent.Period = ShowReportPeriodReportPeriod.ThisMonth;
                    break;
                case ReportPeriod.ThisWeek:
                    intent.Period = ShowReportPeriodReportPeriod.ThisWeek;
                    break;
                case ReportPeriod.ThisYear:
                    intent.Period = ShowReportPeriodReportPeriod.ThisYear;
                    break;
                case ReportPeriod.Unknown:
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

        public void ClearAll()
        {
            INInteraction.DeleteAllInteractions(_ => { });
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