using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public sealed class CollectionChangesListBuilder<TModel>
    {
        private readonly IList<ICollectionChange> changesList = new List<ICollectionChange>();

        public void InsertSection(int index, TModel item)
        {
            changesList.Add(new InsertSectionCollectionChange<TModel>(index, item));
        }

        public void AddRow(SectionedIndex index, TModel item)
        {
            changesList.Add(new AddRowCollectionChange<TModel>(index, item));
        }

        public void RemoveRow(SectionedIndex index)
        {
            changesList.Add(new RemoveRowCollectionChange(index));
        }

        public void UpdateRow(SectionedIndex index, TModel item)
        {
            changesList.Add(new UpdateRowCollectionChange<TModel>(index, item));
        }

        public void MoveRowWithinExistingSections(SectionedIndex oldIndex, SectionedIndex index, TModel item, bool moveToDifferentSection)
        {
            changesList.Add(new MoveRowWithinExistingSectionsCollectionChange<TModel>(oldIndex, index, item, moveToDifferentSection));
        }

        public void MoveRowToNewSection(SectionedIndex oldIndex, int index, TModel item)
        {
            changesList.Add(new MoveRowToNewSectionCollectionChange<TModel>(oldIndex, index, item));
        }

        public void Reload()
        {
            changesList.Add(new ReloadCollectionChange());
        }

        public IReadOnlyList<ICollectionChange> Build()
            => new ReadOnlyCollection<ICollectionChange>(changesList);
    }
}
