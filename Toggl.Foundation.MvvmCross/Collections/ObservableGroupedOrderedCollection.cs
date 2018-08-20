using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Foundation.MvvmCross.Collections.Changes;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.MvvmCross.Collections
{
    public class ObservableGroupedOrderedCollection<TItem> : IGroupOrderedCollection<TItem>
    {
        private ISubject<IReadOnlyCollection<ICollectionChange>> collectionChangesSubject = new Subject<IReadOnlyCollection<ICollectionChange>>();
        private GroupedOrderedCollection<TItem> collection;
        private Func<TItem, IComparable> indexKey;

        public IObservable<IReadOnlyCollection<ICollectionChange>> CollectionChanges
            => collectionChangesSubject.AsObservable();

        public IObservable<bool> Empty
            => collectionChangesSubject
                .AsObservable()
                .Select(_ => IsEmpty)
                .StartWith(IsEmpty)
                .DistinctUntilChanged();

        public IObservable<int> TotalCount
            => collectionChangesSubject
                .AsObservable()
                .Select(_ => collection.TotalCount)
                .StartWith(0)
                .DistinctUntilChanged();

        public bool IsEmpty
            => collection.IsEmpty;

        public int Count
            => collection.Count;

        public ObservableGroupedOrderedCollection(Func<TItem, IComparable> indexKey, Func<TItem, IComparable> orderingKey, Func<TItem, IComparable> groupingKey, bool descending = false)
        {
            this.indexKey = indexKey;
            collection = new GroupedOrderedCollection<TItem>(indexKey, orderingKey, groupingKey, descending);
        }

        public IEnumerator<IReadOnlyList<TItem>> GetEnumerator()
            => collection.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IReadOnlyList<TItem> this[int index]
            => collection[index];

        public SectionedIndex? IndexOf(TItem item)
        {
            return collection.IndexOf(item);
        }

        public SectionedIndex? IndexOf(IComparable itemId)
        {
            return collection.IndexOf(itemId);
        }

        public (SectionedIndex index, bool needsNewSection) InsertItem(TItem item)
        {
            var (index, needsNewSection) = collection.InsertItem(item);

            var changes = new CollectionChangesListBuilder<TItem>();

            if (needsNewSection)
            {
                changes.InsertSection(index.Section, item);
            }
            else
            {
                changes.AddRow(index, item);
            }

            collectionChangesSubject.OnNext(changes.Build());

            return (index, needsNewSection);
        }

        public SectionedIndex? UpdateItem(IComparable key, TItem item)
        {
            var oldIndex = collection.IndexOf(key);
            if (!oldIndex.HasValue)
            {
                return InsertItem(item).index;
            }

            var changes = new CollectionChangesListBuilder<TItem>();

            var section = collection.FitsIntoSection(item);
            var movesToDifferentSection = !section.HasValue || section.Value != oldIndex.Value.Section;
            collection.RemoveItemAt(oldIndex.Value.Section, oldIndex.Value.Row);
            var (newIndex, needsNewSection) = collection.InsertItem(item);

            if (!movesToDifferentSection && oldIndex.Value.Equals(newIndex))
            {
                changes.UpdateRow(newIndex, item);
            }
            else
            {
                if (needsNewSection)
                {
                    changes.MoveRowToNewSection(oldIndex.Value, newIndex.Section, item);
                }
                else
                {
                    changes.MoveRowWithinExistingSections(oldIndex.Value, newIndex, item, movesToDifferentSection);
                }
            }

            collectionChangesSubject.OnNext(changes.Build());
            return newIndex;
        }

        public void ReplaceWith(IEnumerable<TItem> items)
        {
            collection.ReplaceWith(items);

            var changes = new CollectionChangesListBuilder<TItem>();
            changes.Reload();
            collectionChangesSubject.OnNext(changes.Build());
        }

        public TItem RemoveItemAt(int section, int row)
        {
            var index = new SectionedIndex(section, row);
            var item = collection.RemoveItemAt(section, row);

            var changes = new CollectionChangesListBuilder<TItem>();
            changes.RemoveRow(index);
            collectionChangesSubject.OnNext(changes.Build());

            return item;
        }
    }
}
