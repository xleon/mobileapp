using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Foundation;
using MvvmCross.Platforms.Ios.Binding.Views;
using Toggl.Daneel.Cells.Calendar;
using Toggl.Daneel.Views.Calendar;
using Toggl.Foundation.Calendar;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Extensions;
using Toggl.Multivac;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public sealed class CalendarCollectionViewSource : MvxCollectionViewSource, ICalendarCollectionViewLayoutDataSource
    {
        private readonly string itemReuseIdentifier = nameof(CalendarItemView);
        private readonly string hourReuseIdentifier = nameof(HourSupplementaryView);
        private readonly string currentTimeReuseIdentifier = nameof(CurrentTimeSupplementaryView);

        private readonly IObservable<DateTime> date;
        private readonly IObservable<TimeFormat> timeOfDayFormatObservable;
        private readonly ObservableGroupedOrderedCollection<CalendarItem> collection;

        private IList<CalendarItem> calendarItems;
        private IList<CalendarCollectionViewItemLayoutAttributes> layoutAttributes;
        private TimeFormat timeOfDayFormat = TimeFormat.TwelveHoursFormat;
        private DateTime currentDate;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ISubject<CalendarItem> itemTappedSubject = new Subject<CalendarItem>();

        private bool isAddingItem = false;

        public IObservable<CalendarItem> ItemTapped => itemTappedSubject.AsObservable();

        public CalendarCollectionViewSource(
            UICollectionView collectionView,
            IObservable<DateTime> date,
            IObservable<TimeFormat> timeOfDayFormat,
            ObservableGroupedOrderedCollection<CalendarItem> collection)
            : base(collectionView)
        {
            Ensure.Argument.IsNotNull(date, nameof(date));
            Ensure.Argument.IsNotNull(timeOfDayFormat, nameof(timeOfDayFormat));
            Ensure.Argument.IsNotNull(collection, nameof(collection));
            this.date = date;
            this.timeOfDayFormatObservable = timeOfDayFormat;
            this.collection = collection;

            calendarItems = new List<CalendarItem>();
            layoutAttributes = new List<CalendarCollectionViewItemLayoutAttributes>();

            registerCells();

            timeOfDayFormat
                .Subscribe(timeOfDayFormatChanged)
                .DisposedBy(disposeBag);

            collection
                .CollectionChanges
                .ObserveOn(SynchronizationContext.Current)
                .VoidSubscribe(onCollectionChanges)
                .DisposedBy(disposeBag);

            date.Subscribe(dateChanged)
                .DisposedBy(disposeBag);

            onCollectionChanges();
        }

        protected override UICollectionViewCell GetOrCreateCellFor(UICollectionView collectionView, NSIndexPath indexPath, object item)
        {
            var cell = collectionView.DequeueReusableCell(itemReuseIdentifier, indexPath) as CalendarItemView;
            cell.Item = calendarItems[(int)indexPath.Item];
            return cell;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
            => calendarItems.Count;

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = calendarItems[indexPath.Row];
            itemTappedSubject.OnNext(item);
        }

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            if (elementKind == CalendarCollectionViewLayout.HourSupplementaryViewKind)
            {
                var reusableView = collectionView.DequeueReusableSupplementaryView(elementKind, hourReuseIdentifier, indexPath) as HourSupplementaryView;
                var hour = currentDate.AddHours((int)indexPath.Item);
                reusableView.SetLabel(hour.ToString(timeOfDayFormat.Format));
                return reusableView;
            }

            return collectionView.DequeueReusableSupplementaryView(elementKind, currentTimeReuseIdentifier, indexPath);
        }

        public IEnumerable<NSIndexPath> IndexPathsOfCalendarItemsBetweenHours(int minHour, int maxHour)
        {
            if (calendarItems.None())
            {
                return Enumerable.Empty<NSIndexPath>();
            }

            var indices = calendarItems
                .Select((value, index) => new { value, index })
                .Where(t => t.value.StartTime.ToLocalTime().Hour >= minHour && t.value.EndTime.ToLocalTime().Hour <= maxHour)
                .Select(t => t.index);

            return indices.Select(index => NSIndexPath.FromItemSection(index, 0));
        }

        public CalendarCollectionViewItemLayoutAttributes LayoutAttributesForItemAtIndexPath(NSIndexPath indexPath)
        {
            return layoutAttributes[(int)indexPath.Item];
        }

        public NSIndexPath InsertPlaceholder(DateTimeOffset startTime, TimeSpan duration)
        {
            isAddingItem = true;
            var indexPath = insertPlaceholder(startTime, duration);
            CollectionView.InsertItems(new NSIndexPath[] { indexPath });
            return indexPath;
        }

        public NSIndexPath UpdatePlaceholder(NSIndexPath indexPath, DateTimeOffset startTime, TimeSpan duration)
        {
            if (isAddingItem)
            {
                var updatedIndexPath = updatePlaceholder(indexPath, startTime, duration);
                CollectionView.ReloadItems(new NSIndexPath[] { indexPath });
                return updatedIndexPath;
            }
            return indexPath;
        }

        public void RemovePlaceholder(NSIndexPath indexPath)
        {
            if (!isAddingItem)
                return;

            removePlaceholder(indexPath);
            CollectionView.DeleteItems(new NSIndexPath[] { indexPath });
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            disposeBag.Dispose();
        }

        private void timeOfDayFormatChanged(TimeFormat timeFormat)
        {
            timeOfDayFormat = timeFormat;
            CollectionView.ReloadData();
        }

        private void onCollectionChanges()
        {
            calendarItems = collection.IsEmpty ? new List<CalendarItem>() : collection[0].ToList();
            layoutAttributes = calculateLayoutAttributes();
            CollectionView.ReloadData();
        }

        private void dateChanged(DateTime date)
        {
            this.currentDate = date;
        }

        private void registerCells()
        {
            CollectionView.RegisterNibForCell(CalendarItemView.Nib, itemReuseIdentifier);

            CollectionView.RegisterNibForSupplementaryView(HourSupplementaryView.Nib,
                                                           CalendarCollectionViewLayout.HourSupplementaryViewKind,
                                                           new NSString(hourReuseIdentifier));

            CollectionView.RegisterNibForSupplementaryView(CurrentTimeSupplementaryView.Nib,
                                                           CalendarCollectionViewLayout.CurrentTimeSupplementaryViewKind,
                                                           new NSString(currentTimeReuseIdentifier));
        }

        private IList<CalendarCollectionViewItemLayoutAttributes> calculateLayoutAttributes()
        {
            if (calendarItems.None())
                return new List<CalendarCollectionViewItemLayoutAttributes>();

            var seed = new List<List<CalendarItem>>() { new List<CalendarItem>() };

            var attributes = calendarItems
                .Aggregate(seed, (buckets, item) =>
                {
                    var bucket = buckets.Last();
                    if (bucket.None())
                    {
                        bucket.Add(item);
                    }
                    else
                    {
                        var endTime = bucket.Max(i => i.EndTime.LocalDateTime);
                        if (item.StartTime.LocalDateTime < endTime)
                            bucket.Add(item);
                        else
                            buckets.Add(new List<CalendarItem>() { item });
                    }

                    return buckets;
                })
                .Select(bucket => bucket.Select((item, index) => attributesForItem(item, bucket.Count, index)))
                .SelectMany(CommonFunctions.Identity)
                .ToList();

            return attributes;
        }

        private CalendarCollectionViewItemLayoutAttributes attributesForItem(
            CalendarItem calendarItem,
            int overlappingItemsCount,
            int positionInOverlappingGroup)
            => new CalendarCollectionViewItemLayoutAttributes(
                calendarItem.StartTime.LocalDateTime,
                calendarItem.Duration,
                overlappingItemsCount,
                positionInOverlappingGroup
            );

        private NSIndexPath insertPlaceholder(DateTimeOffset startTime, TimeSpan duration)
        {
            var calendarItem = new CalendarItem(CalendarItemSource.TimeEntry, startTime, duration, string.Empty, CalendarIconKind.None);

            var insertPosition = calendarItems.IndexOf(item => item.StartTime > calendarItem.StartTime);
            if (insertPosition >= 0)
            {
                calendarItems.Insert(insertPosition, calendarItem);
            }
            else
            {
                calendarItems.Add(calendarItem);
                insertPosition = calendarItems.Count - 1;
            }

            layoutAttributes = calculateLayoutAttributes();

            var indexPath = NSIndexPath.FromItemSection(insertPosition, 0);
            return indexPath;
        }

        private NSIndexPath updatePlaceholder(NSIndexPath indexPath, DateTimeOffset startTime, TimeSpan duration)
        {
            var oldCalendarItem = calendarItems[(int)indexPath.Item];
            calendarItems.RemoveAt((int)indexPath.Item);

            var calendarItem = oldCalendarItem
                .WithStartTime(startTime)
                .WithDuration(duration);

            var insertPosition = calendarItems.IndexOf(item => item.StartTime > calendarItem.StartTime);
            if (insertPosition >= 0)
            {
                calendarItems.Insert(insertPosition, calendarItem);
            }
            else
            {
                calendarItems.Add(calendarItem);
                insertPosition = calendarItems.Count - 1;
            }

            layoutAttributes = calculateLayoutAttributes();

            var updatedIndexPath = NSIndexPath.FromItemSection(insertPosition, 0);
            return updatedIndexPath;
        }

        private void removePlaceholder(NSIndexPath indexPath)
        {
            calendarItems.RemoveAt((int)indexPath.Item);
            layoutAttributes = calculateLayoutAttributes();
        }
    }
}
