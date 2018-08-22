using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using Toggl.Daneel.ViewSources;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using UIKit;
using Math = System.Math;

namespace Toggl.Daneel.Views.Calendar
{
    public sealed class CalendarCollectionViewCreateFromSpanHelper : NSObject, IUIGestureRecognizerDelegate
    {
        private static readonly TimeSpan defaultDuration = TimeSpan.FromMinutes(15);

        private readonly UICollectionView collectionView;
        private readonly CalendarCollectionViewSource dataSource;
        private readonly CalendarCollectionViewLayout layout;

        private UILongPressGestureRecognizer longPressGestureRecognizer;

        private NSIndexPath itemIndexPath;

        private CGPoint firstPoint;
        private CGPoint lastPoint;

        private readonly ISubject<(DateTimeOffset, TimeSpan)> createFromSpanSuject = new Subject<(DateTimeOffset, TimeSpan)>();
        public IObservable<(DateTimeOffset, TimeSpan)> CreateFromSpan => createFromSpanSuject.AsObservable();

        public CalendarCollectionViewCreateFromSpanHelper(
            UICollectionView collectionView,
            CalendarCollectionViewSource dataSource,
            CalendarCollectionViewLayout layout)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(layout, nameof(layout));

            this.collectionView = collectionView;
            this.dataSource = dataSource;
            this.layout = layout;

            longPressGestureRecognizer = new UILongPressGestureRecognizer(onLongPress);
            longPressGestureRecognizer.Delegate = this;
            collectionView.AddGestureRecognizer(longPressGestureRecognizer);
        }

        public CalendarCollectionViewCreateFromSpanHelper(IntPtr handle) : base(handle)
        {
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
            => otherGestureRecognizer is UILongPressGestureRecognizer;

        private void onLongPress(UILongPressGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(collectionView);

            switch (gesture.State)
            {
                case UIGestureRecognizerState.Began:
                    longPressBegan(point);
                    break;

                case UIGestureRecognizerState.Changed:
                    longPressChanged(point);
                    break;

                case UIGestureRecognizerState.Ended:
                    longPressEnded(point);
                    break;

                case UIGestureRecognizerState.Cancelled:
                case UIGestureRecognizerState.Failed:
                    dataSource.RemoveItemView(itemIndexPath);
                    dataSource.StopEditing();
                    itemIndexPath = null;
                    break;
            }
        }

        private void longPressBegan(CGPoint point)
        {
            if (dataSource.IsEditing || dataSource.CalendarItemAtPoint(point) != null)
                return;

            dataSource.StartEditing();
            firstPoint = point;
            lastPoint = point;
            var startTime = layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();
            itemIndexPath = dataSource.InsertItemView(startTime, defaultDuration);
        }

        private void longPressChanged(CGPoint point)
        {
            if (itemIndexPath == null)
                return;

            if (Math.Abs(lastPoint.Y - point.Y) < layout.HourHeight / 4)
                return;

            lastPoint = point;

            DateTimeOffset startTime;
            DateTimeOffset endTime;

            if (firstPoint.Y < point.Y)
            {
                startTime = layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();
                endTime = layout.DateAtPoint(lastPoint).RoundUpToClosestQuarter();
            }
            else
            {
                startTime = layout.DateAtPoint(lastPoint).RoundDownToClosestQuarter();
                endTime = layout.DateAtPoint(firstPoint).RoundUpToClosestQuarter();
            }

            var duration = endTime - startTime;
            dataSource.UpdateItemView(itemIndexPath, startTime, duration);
        }

        private void longPressEnded(CGPoint point)
        {
            if (itemIndexPath == null)
                return;

            lastPoint = point;

            DateTimeOffset startTime;
            DateTimeOffset endTime;

            if (firstPoint.Y < point.Y)
            {
                startTime = layout.DateAtPoint(firstPoint).RoundDownToClosestQuarter();
                endTime = layout.DateAtPoint(lastPoint).RoundUpToClosestQuarter();
            }
            else
            {
                startTime = layout.DateAtPoint(lastPoint).RoundDownToClosestQuarter();
                endTime = layout.DateAtPoint(firstPoint).RoundUpToClosestQuarter();
            }

            var duration = endTime - startTime;
            createFromSpanSuject.OnNext((startTime, duration));

            dataSource.StopEditing();
            itemIndexPath = null;
        }
    }
}
