using System;
using System.Threading.Tasks;
using CoreGraphics;
using Foundation;
using MvvmCross.Core.ViewModels;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Extensions;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Views
{
    [Register(nameof(MainScrollView))]
    public sealed class MainScrollView : UIScrollView
    {
        internal const float SyncStateViewHeight = 26;
        private static readonly float scrollThreshold = SyncStateViewHeight + (SyncStateViewHeight / 2);

        private readonly UIColor syncingColor = Color.Main.Syncing.ToNativeColor();
        private readonly UIColor syncCompletedColor = Color.Main.SyncCompleted.ToNativeColor();

        private bool isSyncing;
        private bool needsRefresh;
        private bool shouldCalculateOnDeceleration;

        public bool IsSyncing
        {
            get => isSyncing;
            set
            {
                if (value == isSyncing) return;
                isSyncing = value;
                updateViews(value);
            }
        }

        public UIView SyncStateView { get; set; }

        public UILabel SyncStateLabel { get; set; }

        public IMvxCommand RefreshCommand { get; set; }

        public MainScrollView(IntPtr handle)
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Scrolled += onScrolled;
            DraggingEnded += onDragEnded;
            DecelerationEnded += onDecelerationEnded;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;

            Scrolled -= onScrolled;
            DraggingEnded -= onDragEnded;
            DecelerationEnded -= onDecelerationEnded;
        }

        private void onScrolled(object sender, EventArgs e)
        {
            if (!Dragging) return;

            var offset = ContentOffset.Y;
            if (offset >= 0) return;

            var needsMorePulling = Math.Abs(offset) < scrollThreshold;
            needsRefresh = !needsMorePulling;

            if (IsSyncing) return;

            Animate(Animation.Timings.EnterTiming, () =>
                SyncStateLabel.Text = needsMorePulling ? Resources.PullDownToRefresh : Resources.ReleaseToRefresh);
        }

        private void onDragEnded(object sender, DraggingEventArgs e)
        {
            var offset = ContentOffset.Y;
            if (offset >= 0) return;

            shouldCalculateOnDeceleration = e.Decelerate;

            if (shouldCalculateOnDeceleration) return;
            refreshIfNeeded();
        }

        private void onDecelerationEnded(object sender, EventArgs e)
        {
            if (!shouldCalculateOnDeceleration) return;
            refreshIfNeeded();
        }

        private void refreshIfNeeded()
        {
            if (!needsRefresh)
            {
                SetContentOffset(CGPoint.Empty, true);
                return;
            }

            needsRefresh = false;
            shouldCalculateOnDeceleration = false;

            if (IsSyncing)
            {
                SetContentOffset(new CGPoint(0, -SyncStateViewHeight), true);
                return;
            }

            RefreshCommand.Execute();
        }

        private async void updateViews(bool value)
        {
            if (value)
            {
                Animate(Animation.Timings.EnterTiming, () =>
                {
                    SyncStateLabel.Text = Resources.Syncing;
                    SyncStateView.BackgroundColor = syncingColor;
                });

                SetContentOffset(new CGPoint(0, -SyncStateViewHeight), true);
                return;
            }

            Animate(Animation.Timings.EnterTiming, () =>
            {
                SyncStateLabel.AttributedText = Resources.SyncCompleted.EndingWithTick(SyncStateLabel.Font.CapHeight);
                SyncStateView.BackgroundColor = syncCompletedColor;
            });

            await Task.Delay(Animation.Timings.HideSyncStateViewDelay);
            SetContentOffset(CGPoint.Empty, true);
        }
    }
}
