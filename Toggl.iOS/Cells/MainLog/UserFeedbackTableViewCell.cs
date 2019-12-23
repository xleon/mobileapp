using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreGraphics;
using Foundation;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Shared.Extensions;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;
using static Toggl.Shared.Extensions.CommonFunctions;

namespace Toggl.iOS.Cells.MainLog
{
    public partial class UserFeedbackTableViewCell : BaseTableViewCell<UserFeedbackViewModel>
    {
        public static readonly string Identifier = "UserFeedbackTableViewCell";
        public static readonly UINib Nib;

        private readonly UIStringAttributes descriptionStringAttributes = new UIStringAttributes
        {
            ParagraphStyle = new NSMutableParagraphStyle
            {
                MaximumLineHeight = 22,
                MinimumLineHeight = 22,
                Alignment = UITextAlignment.Center,
            }
        };

        private CompositeDisposable disposeBag { get; } = new CompositeDisposable();

        static UserFeedbackTableViewCell()
        {
            Nib = UINib.FromName("UserFeedbackTableViewCell", NSBundle.MainBundle);
        }

        protected UserFeedbackTableViewCell(IntPtr handle) : base(handle)
        {
        }

        protected override void UpdateView()
        {
            Item.RatingViewModel.Impression
                .Select(impression => impression.HasValue)
                .Subscribe(CallToActionView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            Item.RatingViewModel.Impression
                .Select(callToActionTitle)
                .Subscribe(CallToActionTitle.Rx().Text())
                .DisposedBy(disposeBag);

            Item.RatingViewModel.Impression
                .Select(callToActionDescription)
                .Select(attributedDescription)
                .Subscribe(CallToActionDescription.Rx().AttributedText())
                .DisposedBy(disposeBag);

            Item.RatingViewModel.Impression
                .Select(callToActionButtonTitle)
                .Subscribe(CallToActionButton.Rx().Title())
                .DisposedBy(disposeBag);

            Item.RatingViewModel.Impression
                .Select(impression => impression.HasValue)
                .Select(Invert)
                .Subscribe(QuestionView.Rx().IsVisibleWithFade())
                .DisposedBy(disposeBag);

            YesView.Rx().Tap()
                .Subscribe(() => Item.RatingViewModel.RegisterImpression(true))
                .DisposedBy(disposeBag);

            NotReallyView.Rx().Tap()
                .Subscribe(() => Item.RatingViewModel.RegisterImpression(false))
                .DisposedBy(disposeBag);

            CallToActionButton.Rx()
                .BindAction(Item.RatingViewModel.PerformMainAction)
                .DisposedBy(disposeBag);

            DismissButton.Rx().Tap()
                .Subscribe(Item.RatingViewModel.Dismiss)
                .DisposedBy(disposeBag);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag.Dispose();
        }

        public override void LayoutIfNeeded()
        {
            base.LayoutIfNeeded();

            TitleLabel.Text = Resources.RatingTitle;
            YesLabel.Text = Resources.RatingYes;
            NotReallyLabel.Text = Resources.RatingNotReally;

            CallToActionButton.SetTitle(Resources.RatingCallToActionTitle, UIControlState.Normal);
            DismissButton.SetTitle(Resources.NoThanks, UIControlState.Normal);

            QuestionView.UpdateCardView();
            CallToActionView.UpdateCardView();

            CallToActionButton.Layer.CornerRadius = 8;
            CallToActionView.Layer.MasksToBounds = false;
        }

        private NSAttributedString attributedDescription(string text)
            => new NSAttributedString(text, descriptionStringAttributes);

        private string callToActionTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Resources.RatingViewPositiveCallToActionTitle
                   : Resources.RatingViewNegativeCallToActionTitle;
        }

        private string callToActionDescription(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? string.Format(Resources.RatingViewPositiveCallToActionDescription, Resources.IosStoreName)
                   : Resources.RatingViewNegativeCallToActionDescription;
        }

        private string callToActionButtonTitle(bool? impressionIsPositive)
        {
            if (impressionIsPositive == null)
                return string.Empty;

            return impressionIsPositive.Value
                   ? Resources.RatingViewPositiveCallToActionButtonTitle
                   : Resources.RatingViewNegativeCallToActionButtonTitle;
        }
    }
}

