using System.Reactive.Linq;
using System.Threading.Tasks;
using MvvmCross.Plugin.Color.Platforms.Ios;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Multivac.Extensions;
using UIKit;
using static Toggl.Multivac.Extensions.CommonFunctions;

namespace Toggl.Daneel.ViewControllers.Settings
{
    [ModalCardPresentation]
    public sealed partial class SendFeedbackViewController : ReactiveViewController<SendFeedbackViewModel>, IDismissableViewController
    {

        public SendFeedbackViewController()
            : base(nameof(SendFeedbackViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();
            prepareIndicatorView();

            this.Bind(CloseButton.Rx().Tap(), ViewModel.CloseButtonTapped);
            this.Bind(FeedbackTextView.Rx().Text(), ViewModel.FeedbackText);
            this.Bind(ErrorView.Rx().Tap(), ViewModel.ErrorViewTapped);

            this.Bind(SendButton.Rx().Tap(), ViewModel.SendButtonTapped);
            SendButton.TouchUpInside += (sender, args) => { FeedbackTextView.ResignFirstResponder(); };

            this.Bind(ViewModel.IsFeedbackEmpty, FeedbackPlaceholderTextView.Rx().IsVisible());
            this.Bind(ViewModel.Error.Select(NotNull), ErrorView.Rx().AnimatedIsVisible());
            this.Bind(ViewModel.SendEnabled, SendButton.Rx().Enabled());

            this.Bind(ViewModel.IsLoading.Invert(), SendButton.Rx().IsVisible());
            this.Bind(ViewModel.IsLoading.Invert(), CloseButton.Rx().IsVisible());
            this.Bind(ViewModel.IsLoading, IndicatorView.Rx().IsVisible());
            this.Bind(ViewModel.IsLoading, UIApplication.SharedApplication.Rx().NetworkActivityIndicatorVisible());
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            IndicatorView.StartSpinning();
        }

        public async Task<bool> Dismiss()
        {
            await ViewModel.CloseButtonTapped.Execute();
            return true;
        }

        private void prepareViews()
        {
            ErrorView.Hidden = true;
            FeedbackTextView.TintColor = Color.Feedback.Cursor.ToNativeColor();
            FeedbackPlaceholderTextView.TintColor = Color.Feedback.Cursor.ToNativeColor();
        }

        private void prepareIndicatorView()
        {
            IndicatorView.IndicatorColor = Color.Feedback.ActivityIndicator.ToNativeColor();
            IndicatorView.Hidden = true;
        }
    }
}

