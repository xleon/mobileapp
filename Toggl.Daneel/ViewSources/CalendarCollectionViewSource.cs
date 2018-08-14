using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
    public sealed class CalendarCollectionViewSource
        : MvxCollectionViewSource, ICalendarCollectionViewLayoutDataSource
    {
        private readonly string itemReuseIdentifier = nameof(CalendarItemView);
        private readonly string hourReuseIdentifier = nameof(HourSupplementaryView);
        private readonly string currentTimeReuseIdentifier = nameof(CurrentTimeSupplementaryView);

        private readonly ObservableGroupedOrderedCollection<CalendarItem> collection;
        private IList<CalendarItem> calendarItems;
        private IList<CalendarCollectionViewItemLayoutAttributes> layoutAttributes;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public CalendarCollectionViewSource(UICollectionView collectionView, ObservableGroupedOrderedCollection<CalendarItem> collection)
            : base(collectionView)
        {
            Ensure.Argument.IsNotNull(collection, nameof(collection));
            this.collection = collection;

            calendarItems = new List<CalendarItem>();
            layoutAttributes = new List<CalendarCollectionViewItemLayoutAttributes>();

            registerCells();

            collection
                .CollectionChanges
                .ObserveOn(SynchronizationContext.Current)
                .VoidSubscribe(onCollectionChanges)
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

        [Export("collectionView:viewForSupplementaryElementOfKind:atIndexPath:")]
        public UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            if (elementKind == CalendarCollectionViewLayout.HourSupplementaryViewKind)
            {
                var reusableView = collectionView.DequeueReusableSupplementaryView(elementKind, hourReuseIdentifier, indexPath) as HourSupplementaryView;
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
                .Where(t => t.value.StartTime.Hour >= minHour && t.value.EndTime.Hour <= maxHour)
                .Select(t => t.index);

            return indices.Select(index => NSIndexPath.FromItemSection(index, 0));
        }

        public CalendarCollectionViewItemLayoutAttributes LayoutAttributesForItemAtIndexPath(NSIndexPath indexPath)
        {
            return layoutAttributes[(int)indexPath.Item];
        }

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (!isDisposing) return;

            disposeBag.Dispose();
        }

        private void onCollectionChanges()
        {
            calendarItems = collection.IsEmpty ? new List<CalendarItem>() : collection[0].ToList();
            layoutAttributes = calculateLayoutAttributes();
            CollectionView.ReloadData();
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

            var firstItem = calendarItems[0];

            var seed = new List<List<CalendarItem>>() { new List<CalendarItem>() };

            var attributes = calendarItems
                .Aggregate(seed, (buckets, item) =>
                {
                    var bucket = buckets.Last();
                    var endTime = bucket.Any() ? bucket.Last().EndTime : default(DateTime);
                    if (item.StartTime <= endTime)
                        bucket.Add(item);
                    else
                        buckets.Add(new List<CalendarItem>() { item });

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
                calendarItem.StartTime.DateTime,
                calendarItem.Duration,
                overlappingItemsCount,
                positionInOverlappingGroup
            );
    }
}
