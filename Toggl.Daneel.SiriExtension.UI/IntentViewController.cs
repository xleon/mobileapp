using System;
using CoreGraphics;
using Foundation;
using Intents;
using IntentsUI;
using UIKit;
using CoreFoundation;
using Toggl.Daneel.Intents;

namespace Toggl.Daneel.SiriExtension.UI
{
    public partial class IntentViewController : UIViewController, IINUIHostedViewControlling
    {
        protected IntentViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        [Export("configureViewForParameters:ofInteraction:interactiveBehavior:context:completion:")]
        public void ConfigureView(
                 NSSet<INParameter> parameters,
                 INInteraction interaction,
                 INUIInteractiveBehavior interactiveBehavior,
                 INUIHostedViewContext context,
                 INUIHostedViewControllingConfigureViewHandler completion)
        {

            var intent = interaction.Intent as StopTimerIntent;
            if (intent is null)
            {
                completion(false, new NSSet<INParameter>(), CGSize.Empty);
            }

            var desiredSize = CGSize.Empty;
                      
            if (interaction.IntentHandlingStatus == INIntentHandlingStatus.Success)
            {
                Console.WriteLine("Success");
                var response = interaction.IntentResponse as StopTimerIntentResponse;
                if (!(response is null))
                {
                    showStopResponse(response);
                    desiredSize = new CGSize(300, 60);
                }
            }

            completion(true, parameters, new CGSize(300, 60));
        }

        private void showStopResponse(StopTimerIntentResponse response)
        {
            descriptionLabel.Text = response.EntryDescription;

            var timeSpan = TimeSpan.FromSeconds(response.EntryDuration.DoubleValue);
            timeLabel.Text = timeSpan.ToString(@"hh\:mm\:ss");

            var startTime = DateTimeOffset.FromUnixTimeSeconds(response.EntryStart.LongValue).ToLocalTime();
            var endTime = DateTimeOffset.FromUnixTimeSeconds(response.EntryStart.LongValue + response.EntryDuration.LongValue).ToLocalTime();
            var fromTime = startTime.ToString("HH:mm");
            var toTime = endTime.ToString("HH:mm");
            timeFrameLabel.Text = $"{fromTime} - {toTime}";
        }

        [Export("configureWithInteraction:context:completion:")]
        public void Configure(INInteraction interaction, INUIHostedViewContext context, Action<CGSize> completion)
        {
            throw new NotImplementedException();
        }
    }
}
