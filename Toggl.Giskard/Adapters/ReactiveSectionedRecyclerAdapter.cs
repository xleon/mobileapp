using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Android.Support.V7.Widget;
using Android.Views;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Collections.Changes;
using Toggl.Giskard.ViewHolders;

namespace Toggl.Giskard.Adapters
{
    public abstract class ReactiveSectionedRecyclerAdapter<TModel, TItemViewHolder, TSectionViewHolder> : RecyclerView.Adapter
        where TItemViewHolder : BaseRecyclerViewHolder<TModel>
        where TSectionViewHolder : BaseRecyclerViewHolder<IReadOnlyList<TModel>>
    {
        public const int SectionViewType = 0;
        public const int ItemViewType = 1;

        public virtual int HeaderOffset { get; } = 0;

        private readonly ObservableGroupedOrderedCollection<TModel> items;
        private ImmutableList<FlattenedItemInfo> collectionToAdapterIndexesMap = ImmutableList<FlattenedItemInfo>.Empty;
        private ImmutableList<int> sectionsIndexes = ImmutableList<int>.Empty;

        public ReactiveSectionedRecyclerAdapter(ObservableGroupedOrderedCollection<TModel> items)
        {
            this.items = items;
            updateSectionIndexes();
        }

        public override int ItemCount => collectionToAdapterIndexesMap.Count + HeaderOffset;

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == ItemViewType) return CreateItemViewHolder(parent);
            return CreateHeaderViewHolder(parent);
        }

        public override int GetItemViewType(int position)
        {
            if (sectionsIndexes.Contains(position - HeaderOffset)) return SectionViewType;
            return ItemViewType;
        }

        public void UpdateChange(ICollectionChange change)
        {
            switch (change)
            {
                case AddRowCollectionChange<TModel> addRow:
                    updateSectionIndexes();
                    NotifyItemInserted(mapSectionIndexToAdapterIndex(addRow.Index));
                    NotifyItemChanged(sectionsIndexes[addRow.Index.Section] + HeaderOffset);
                    break;

                case InsertSectionCollectionChange<TModel> insertSection:
                    updateSectionIndexes();
                    NotifyItemRangeInserted(sectionsIndexes[insertSection.Index] + HeaderOffset, 2);
                    break;

                case UpdateRowCollectionChange<TModel> updateRow:
                    NotifyItemChanged(mapSectionIndexToAdapterIndex(updateRow.Index));
                    NotifyItemChanged(sectionsIndexes[updateRow.Index.Section] + HeaderOffset);
                    break;

                case MoveRowWithinExistingSectionsCollectionChange<TModel> moveRowWithinExistingSectionsCollectionChange:
                    moveRowWithinExistingSections(moveRowWithinExistingSectionsCollectionChange);
                    break;

                case MoveRowToNewSectionCollectionChange<TModel> moveRowToNewSection:
                    this.moveRowToNewSection(moveRowToNewSection);
                    break;

                case RemoveRowCollectionChange removeRow:
                    this.removeRow(removeRow);
                    break;

                case ReloadCollectionChange reload:
                    updateSectionIndexes();
                    NotifyDataSetChanged();
                    break;
            }
        }

        public TModel GetItemFromAdapterPosition(int position)
        {
            var item = collectionToAdapterIndexesMap[position - HeaderOffset];
            if (item.ViewType == ItemViewType)
            {
                return items[item.SectionedIndex.Section][item.SectionedIndex.Row];
            }

            throw new InvalidOperationException($"This position does not contain an item of the {typeof(TModel).Name} type");
        }

        public IReadOnlyList<TModel> GetSectionListFromAdapterPosition(int position)
        {
            var item = collectionToAdapterIndexesMap[position - HeaderOffset];
            if (item.ViewType == SectionViewType)
            {
                return items[item.SectionedIndex.Section];
            }

            throw new InvalidOperationException($"This position does not contain an item of the {typeof(IReadOnlyList<TModel>).Name} type");
        }

        public sealed override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            switch (holder)
            {
                case TItemViewHolder itemViewHolder:
                    itemViewHolder.Item = GetItemFromAdapterPosition(position);
                    break;

                case TSectionViewHolder sectionViewHolder:
                    sectionViewHolder.Item = GetSectionListFromAdapterPosition(position);
                    break;

                default:
                    if (TryBindCustomViewType(holder, position) == false)
                    {
                        throw new InvalidOperationException($"{holder.GetType().Name} was not bound to position {position}");
                    }

                    break;
            }
        }

        protected abstract bool TryBindCustomViewType(RecyclerView.ViewHolder holder, int position);

        protected abstract TSectionViewHolder CreateHeaderViewHolder(ViewGroup parent);

        protected abstract TItemViewHolder CreateItemViewHolder(ViewGroup parent);

        private int mapSectionIndexToAdapterIndex(SectionedIndex sectionedIndex)
        {
            return sectionsIndexes[sectionedIndex.Section] + sectionedIndex.Row + 1 + HeaderOffset;
        }

        private void updateSectionIndexes()
        {
            if (items.IsEmpty)
            {
                collectionToAdapterIndexesMap = ImmutableList<FlattenedItemInfo>.Empty;
                sectionsIndexes = ImmutableList<int>.Empty;
            }
            else
            {
                var mappedIndexes = new List<FlattenedItemInfo>();
                var newSectionsIndexes = new List<int>();
                var sectionIndex = 0;
                var sectionIndexOnCollection = 0;
                foreach (var section in items)
                {
                    newSectionsIndexes.Add(sectionIndex);
                    mappedIndexes.Add(new FlattenedItemInfo(SectionViewType, new SectionedIndex(sectionIndexOnCollection, 0)));
                    mappedIndexes.AddRange(Enumerable.Range(0, section.Count).Select(
                        itemIndex => new FlattenedItemInfo(ItemViewType, new SectionedIndex(sectionIndexOnCollection, itemIndex)))
                    );
                    sectionIndex += section.Count + 1;
                    sectionIndexOnCollection++;
                }

                sectionsIndexes = newSectionsIndexes.ToImmutableList();
                collectionToAdapterIndexesMap = mappedIndexes.ToImmutableList();
            }
        }

        private void removeRow(RemoveRowCollectionChange removeRow)
        {
            var indexToBeRemoved = mapSectionIndexToAdapterIndex(removeRow.Index);
            var sectionCountBefore = sectionsIndexes.Count;
            updateSectionIndexes();
            if (sectionCountBefore == sectionsIndexes.Count)
            {
                NotifyItemRemoved(indexToBeRemoved);
                NotifyItemChanged(sectionsIndexes[removeRow.Index.Section] + HeaderOffset);
            }
            else
            {
                NotifyItemRangeRemoved(indexToBeRemoved - 1, 2);
            }
        }

        private void moveRowToNewSection(MoveRowToNewSectionCollectionChange<TModel> moveRowToNewSection)
        {
            var oldIndexToBeRemoved = mapSectionIndexToAdapterIndex(moveRowToNewSection.OldIndex);
            var sectionsCountBeforeUpdate = sectionsIndexes.Count;
            updateSectionIndexes();

            var oldSectionWasDeleted = sectionsCountBeforeUpdate == sectionsIndexes.Count;
            if (oldSectionWasDeleted)
            {
                NotifyItemRangeRemoved(oldIndexToBeRemoved - 1, 2);
            }
            else
            {
                NotifyItemRemoved(oldIndexToBeRemoved);
            }

            var newSectionIndex = sectionsIndexes[moveRowToNewSection.Index] + HeaderOffset;
            NotifyItemRangeInserted(newSectionIndex, 2);
        }

        private void moveRowWithinExistingSections(MoveRowWithinExistingSectionsCollectionChange<TModel> moveRowWithinExistingSections)
        {
            var sectionsCountBeforeUpdate = sectionsIndexes.Count;
            var oldIndex = mapSectionIndexToAdapterIndex(moveRowWithinExistingSections.OldIndex);
            var oldSectionIndex = sectionsIndexes[moveRowWithinExistingSections.OldIndex.Section] + HeaderOffset;

            updateSectionIndexes();
            var oldSectionWasDeleted = sectionsCountBeforeUpdate != sectionsIndexes.Count;
            if (oldSectionWasDeleted)
            {
                NotifyItemRemoved(oldSectionIndex);
            }

            var newIndex = mapSectionIndexToAdapterIndex(moveRowWithinExistingSections.Index);
            NotifyItemMoved(oldIndex, newIndex);
        }

        private struct FlattenedItemInfo
        {
            public int ViewType { get; }
            public SectionedIndex SectionedIndex { get; }

            public FlattenedItemInfo(int viewType, SectionedIndex sectionedIndex)
            {
                ViewType = viewType;
                SectionedIndex = sectionedIndex;
            }
        }
    }
}
