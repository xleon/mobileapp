using System;
using System.Collections.Immutable;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CoreGraphics;
using Foundation;
using Toggl.iOS.Views.Reports;
using Toggl.Shared;
using UIKit;
using static Toggl.Core.UI.ViewModels.DateRangePicker.DateRangePickerViewModel;

namespace Toggl.iOS.ViewSources
{
    public class DateRangePickerShortcutsSource
    : UICollectionViewSource, IUICollectionViewDelegateFlowLayout
    {
        private const int cellWidth = 96;
        private const int cellHeight = 32;
        private const string cellIdentifier = nameof(DateRangePickerShortcutCell);

        private readonly ISubject<Shortcut> shortcutTaps = new Subject<Shortcut>();
        private readonly UICollectionView collectionView;
        private IImmutableList<Shortcut> shortcuts = ImmutableList<Shortcut>.Empty;

        public IObservable<Shortcut> ShortcutTaps { get; }

        public DateRangePickerShortcutsSource(UICollectionView collectionView)
        {
            Ensure.Argument.IsNotNull(collectionView, nameof(collectionView));

            collectionView.RegisterNibForCell(DateRangePickerShortcutCell.Nib, cellIdentifier);
            ShortcutTaps = shortcutTaps.AsObservable();
            this.collectionView = collectionView;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var item = shortcuts[indexPath.Row];
            var cell = collectionView.DequeueReusableCell(cellIdentifier, indexPath) as DateRangePickerShortcutCell;

            cell.Item = item;

            return cell;
        }

        public override nint NumberOfSections(UICollectionView collectionView) => 1;

        public override nint GetItemsCount(UICollectionView collectionView, nint section) => shortcuts.Count;

        [Export("collectionView:layout:sizeForItemAtIndexPath:")]
        public CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
        {
            return new CGSize(cellWidth, cellHeight);
        }

        public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
        {
            shortcutTaps.OnNext(shortcuts[indexPath.Row]);
        }

        public void UpdateShortcuts(IImmutableList<Shortcut> newShortcuts)
        {
            shortcuts = newShortcuts;
            collectionView.ReloadData();
        }
    }
}
