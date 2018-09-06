using System;
using System.Linq;
using System.Reactive.Subjects;
using Android.Animation;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Foundation.MvvmCross.Transformations;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Extensions;

namespace Toggl.Giskard.ViewHolders
{
    public class MainLogCellViewHolder : BaseRecyclerViewHolder<TimeEntryViewModel>
    {
        public MainLogCellViewHolder(View itemView) : base(itemView)
        {
        }

        public MainLogCellViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        public enum AnimationSide
        {
            Left,
            Right
        }

        public Subject<TimeEntryViewModel> ContinueButtonTappedSubject { get; set; }

        private static readonly int animationDuration = 1000;
        private ObjectAnimator animator;

        public bool IsAnimating => animator?.IsRunning ?? false;

        private TextView timeEntriesLogCellDescription;
        private TextView addDescriptionLabel;
        private TextView timeEntriesLogCellProjectLabel;
        private TextView timeEntriesLogCellTaskLabel;
        private TextView timeEntryLogCellClientLabel;
        private TextView timeEntriesLogCellDuration;
        private View timeEntriesLogCellContinueImage;
        private View errorImageView;
        private View errorNeedsSync;
        private View timeEntriesLogCellContinueButton;
        private View mainLogBackgroundContinue;
        private View mainLogBackgroundDelete;
        public View MainLogContentView { get; private set; }

        protected override void InitializeViews()
        {
            timeEntriesLogCellDescription = ItemView.FindViewById<TextView>(Resource.Id.TimeEntriesLogCellDescription);
            addDescriptionLabel = ItemView.FindViewById<TextView>(Resource.Id.AddDescriptionLabel);
            timeEntriesLogCellProjectLabel = ItemView.FindViewById<TextView>(Resource.Id.TimeEntriesLogCellProjectLabel);
            timeEntriesLogCellTaskLabel = ItemView.FindViewById<TextView>(Resource.Id.TimeEntriesLogCellTaskLabel);
            timeEntryLogCellClientLabel = ItemView.FindViewById<TextView>(Resource.Id.TimeEntryLogCellClientLabel);
            timeEntriesLogCellDuration = ItemView.FindViewById<TextView>(Resource.Id.TimeEntriesLogCellDuration);
            timeEntriesLogCellContinueImage = ItemView.FindViewById(Resource.Id.TimeEntriesLogCellContinueImage);
            errorImageView = ItemView.FindViewById(Resource.Id.ErrorImageView);
            errorNeedsSync = ItemView.FindViewById(Resource.Id.ErrorNeedsSync);
            timeEntriesLogCellContinueButton = ItemView.FindViewById(Resource.Id.TimeEntriesLogCellContinueButton);
            mainLogBackgroundContinue = ItemView.FindViewById(Resource.Id.MainLogBackgroundContinue);
            mainLogBackgroundDelete = ItemView.FindViewById(Resource.Id.MainLogBackgroundDelete);
            timeEntriesLogCellContinueButton.Click += onContinueClick;
            MainLogContentView = ItemView.FindViewById(Resource.Id.MainLogContentView);
        }

        public void ShowSwipeToContinueBackground()
        {
            StopAnimating();
            mainLogBackgroundContinue.Visibility = ViewStates.Visible;
            mainLogBackgroundDelete.Visibility = ViewStates.Invisible;
        }

        public void ShowSwipeToDeleteBackground()
        {
            StopAnimating();
            mainLogBackgroundContinue.Visibility = ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = ViewStates.Visible;
        }

        public void HideSwipeBackgrounds()
        {
            StopAnimating();
            mainLogBackgroundContinue.Visibility = ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = ViewStates.Invisible;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing || timeEntriesLogCellContinueButton == null) return;
            timeEntriesLogCellContinueButton.Click -= onContinueClick;
        }

        private void onContinueClick(object sender, EventArgs e)
        {
            ContinueButtonTappedSubject?.OnNext(Item);
        }

        protected override void UpdateView()
        {
            StopAnimating();
            timeEntriesLogCellDescription.Text = Item.Description;
            timeEntriesLogCellDescription.Visibility = Item.HasDescription.ToVisibility(true);

            addDescriptionLabel.Visibility = (!Item.HasDescription).ToVisibility(true);

            timeEntriesLogCellProjectLabel.Text = Item.ProjectName;
            timeEntriesLogCellProjectLabel.SetTextColor(Color.ParseColor(Item.ProjectColor));
            timeEntriesLogCellProjectLabel.Visibility = Item.HasProject.ToVisibility(true);

            timeEntriesLogCellTaskLabel.Text = $": {Item.TaskName}";
            timeEntriesLogCellTaskLabel.SetTextColor(Color.ParseColor(Item.ProjectColor));
            timeEntriesLogCellTaskLabel.Visibility = (!string.IsNullOrEmpty(Item.TaskName)).ToVisibility(true);

            timeEntryLogCellClientLabel.Text = Item.ClientName;
            timeEntryLogCellClientLabel.Visibility = Item.HasProject.ToVisibility(true);

            timeEntriesLogCellDuration.Text = Item.Duration.HasValue
                ? DurationAndFormatToString.Convert(Item.Duration.Value, Item.DurationFormat)
                : "";

            timeEntriesLogCellContinueImage.Visibility = Item.CanSync.ToVisibility(true);
            errorImageView.Visibility = (!Item.CanSync).ToVisibility(true);
            errorNeedsSync.Visibility = Item.NeedsSync.ToVisibility(true);
            timeEntriesLogCellContinueButton.Visibility = Item.CanSync.ToVisibility(true);
        }

        public bool CanSync => Item.CanSync;

        public void StartAnimating(AnimationSide side)
        {
            if (animator != null && animator.IsRunning)
                return;

            mainLogBackgroundContinue.Visibility = side == AnimationSide.Right ? ViewStates.Visible : ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = side == AnimationSide.Left ? ViewStates.Visible : ViewStates.Invisible;

            var offsetsInDp = getAnimationOffsetsForSide(side);
            var offsetsInPx = offsetsInDp.Select(offset => (float)offset.DpToPixels(ItemView.Context)).ToArray();

            animator = ObjectAnimator.OfFloat(MainLogContentView, "translationX", offsetsInPx);
            animator.SetDuration(animationDuration);
            animator.RepeatMode = ValueAnimatorRepeatMode.Reverse;
            animator.RepeatCount = ValueAnimator.Infinite;
            animator.Start();
        }

        public void StopAnimating()
        {
            if (animator != null)
            {
                animator.Cancel();
                animator = null;
            }

            MainLogContentView.TranslationX = 0;
            mainLogBackgroundContinue.Visibility = ViewStates.Invisible;
            mainLogBackgroundDelete.Visibility = ViewStates.Invisible;
        }

        private float[] getAnimationOffsetsForSide(AnimationSide side)
        {
            switch (side)
            {
                case AnimationSide.Right:
                    return new[] { 50, 0, 3.5f, 0 };
                case AnimationSide.Left:
                    return new[] { -50, 0, -3.5f, 0 };
                default:
                    throw new ArgumentException("Unexpected side");
            }
        }
    }
}
