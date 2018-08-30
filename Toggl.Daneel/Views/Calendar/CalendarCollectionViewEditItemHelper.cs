using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using Toggl.Daneel.Cells.Calendar;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.Extensions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using UIKit;
using Math = System.Math;

namespace Toggl.Daneel.Views.Calendar
{
    public sealed class CalendarCollectionViewEditItemHelper : NSObject, IUIGestureRecognizerDelegate
    {
        enum EditAction
        {
            ChangeOffset,
            ChangeStartTime,
            ChangeEndTime,
            None
        }

        private static readonly TimeSpan defaultDuration = TimeSpan.FromMinutes(15);

        private readonly UICollectionView collectionView;
        private readonly CalendarCollectionViewSource dataSource;
        private readonly CalendarCollectionViewLayout layout;

        private UILongPressGestureRecognizer longPressGestureRecognizer;
        private UIPanGestureRecognizer panGestureRecognizer;
        private UITapGestureRecognizer tapGestureRecognizer;

        private CalendarItem calendarItem;

        private NSIndexPath itemIndexPath;
        private nfloat verticalOffset;
        private CGPoint firstPoint;
        private CGPoint lastPoint;

        private bool isActive;
        private EditAction action;

        private readonly ISubject<CalendarItem> editCalendarItemSuject = new Subject<CalendarItem>();
        public IObservable<CalendarItem> EditCalendarItem => editCalendarItemSuject.AsObservable();

        public CalendarCollectionViewEditItemHelper(
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

            panGestureRecognizer = new UIPanGestureRecognizer(onPan);
            panGestureRecognizer.Delegate = this;

            tapGestureRecognizer = new UITapGestureRecognizer(onTap);
            tapGestureRecognizer.Delegate = this;
        }

        public CalendarCollectionViewEditItemHelper(IntPtr handle) : base(handle)
        {
        }

        [Export("gestureRecognizer:shouldRecognizeSimultaneouslyWithGestureRecognizer:")]
        public bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
        {
            if (gestureRecognizer == longPressGestureRecognizer)
                return otherGestureRecognizer is UILongPressGestureRecognizer;
            else
                return false;
        }

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
                    longPressEnded();
                    break;
            }
        }

        private void onPan(UIPanGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(collectionView);

            switch (gesture.State)
            {
                case UIGestureRecognizerState.Began:
                    panBegan(point);
                    break;

                case UIGestureRecognizerState.Changed:
                    panChanged(point);
                    break;
            }
        }

        private void onTap(UITapGestureRecognizer gesture)
        {
            var point = gesture.LocationInView(collectionView);

            resignActive();
            dataSource.StopEditing();
            editCalendarItemSuject.OnNext(calendarItem);
        }

        private void longPressBegan(CGPoint point)
        {
            if (dataSource.IsEditing || isActive)
                return;

            calendarItem = dataSource.CalendarItemAtPoint(point).Value;
            if (!calendarItem.IsEditable())
                return;

            itemIndexPath = collectionView.IndexPathForItemAtPoint(point);
            dataSource.StartEditing(itemIndexPath);
            becomeActive();

            var startPoint = layout.PointAtDate(calendarItem.StartTime.ToLocalTime());
            firstPoint = point;
            lastPoint = point;
            verticalOffset = firstPoint.Y - startPoint.Y;
        }

        private void longPressChanged(CGPoint point)
        {
            action = EditAction.ChangeOffset;
            changeOffset(point);
        }

        private void longPressEnded()
        {
            if (!isActive)
                return;

            collectionView.RemoveGestureRecognizer(longPressGestureRecognizer);
        }

        private void panBegan(CGPoint point)
        {
            if (!isActive)
                return;

            firstPoint = point;
            lastPoint = point;

            var cell = collectionView.CellForItem(itemIndexPath) as CalendarItemView;
            var topDragRect = collectionView.ConvertRectFromView(cell.TopDragTouchArea, cell);
            var bottomDragRect = collectionView.ConvertRectFromView(cell.BottomDragTouchArea, cell);

            if (topDragRect.Contains(point))
                action = EditAction.ChangeStartTime;
            else if (bottomDragRect.Contains(point))
                action = EditAction.ChangeEndTime;
            else if (cell.Frame.Contains(point))
                action = EditAction.ChangeOffset;
            else
                action = EditAction.None;
        }

        private void panChanged(CGPoint point)
        {
            switch (action)
            {
                case EditAction.ChangeOffset:
                    changeOffset(point);
                    break;
                case EditAction.ChangeStartTime:
                    changeStartTime(point);
                    break;
                case EditAction.ChangeEndTime:
                    changeEndTime(point);
                    break;
            }
        }

        private void changeOffset(CGPoint point)
        {
            if (!isActive || itemIndexPath == null)
                return;

            if (Math.Abs(lastPoint.Y - point.Y) < layout.HourHeight / 4)
                return;

            lastPoint = point;
            var startPoint = new CGPoint(lastPoint.X, lastPoint.Y - verticalOffset);
            var startTime = layout.DateAtPoint(startPoint).ToLocalTime().RoundDownToClosestQuarter();

            if (startTime + calendarItem.Duration > startTime.Date.AddDays(1))
                return;

            calendarItem = calendarItem
                .WithStartTime(startTime);

            itemIndexPath = dataSource.UpdateItemView(itemIndexPath, calendarItem.StartTime, calendarItem.Duration);
        }

        private void changeStartTime(CGPoint point)
        {
            if (!isActive || itemIndexPath == null)
                return;

            if (Math.Abs(lastPoint.Y - point.Y) < layout.HourHeight / 4)
                return;

            if (point.Y < 0 || point.Y >= layout.ContentViewHeight)
                return;

            lastPoint = point;
            var startTime = layout.DateAtPoint(lastPoint).ToLocalTime().RoundDownToClosestQuarter();
            var duration = calendarItem.EndTime - startTime;

            if (duration <= TimeSpan.Zero)
                return;

            calendarItem = calendarItem
                .WithStartTime(startTime)
                .WithDuration(duration);

            itemIndexPath = dataSource.UpdateItemView(itemIndexPath, calendarItem.StartTime, calendarItem.Duration);
        }

        private void changeEndTime(CGPoint point)
        {
            if (!isActive || itemIndexPath == null)
                return;

            if (Math.Abs(lastPoint.Y - point.Y) < layout.HourHeight / 4)
                return;

            if (point.Y < 0 || point.Y >= layout.ContentViewHeight)
                return;

            lastPoint = point;
            var endTime = layout.DateAtPoint(lastPoint).ToLocalTime().RoundUpToClosestQuarter();
            var duration = endTime - calendarItem.StartTime;

            if (duration <= TimeSpan.Zero)
                return;

            calendarItem = calendarItem
                .WithDuration(duration);

            itemIndexPath = dataSource.UpdateItemView(itemIndexPath, calendarItem.StartTime, calendarItem.Duration);
        }

        private void becomeActive()
        {
            isActive = true;
            collectionView.AddGestureRecognizer(panGestureRecognizer);
            collectionView.AddGestureRecognizer(tapGestureRecognizer);
        }

        private void resignActive()
        {
            isActive = false;
            collectionView.AddGestureRecognizer(longPressGestureRecognizer);
            collectionView.RemoveGestureRecognizer(panGestureRecognizer);
            collectionView.RemoveGestureRecognizer(tapGestureRecognizer);
        }
    }
}
