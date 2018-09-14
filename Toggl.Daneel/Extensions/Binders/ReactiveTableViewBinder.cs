using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using Foundation;
using Toggl.Daneel.Cells;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Collections.Changes;
using Toggl.Multivac.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewSources
{
    public class ReactiveTableViewBinder<TModel, TCell> : IDisposable
        where TCell : BaseTableViewCell<TModel>
    {
        private readonly object animationLock = new object();
        private CompositeDisposable disposeBag = new CompositeDisposable();

        private ReactiveSectionedListTableViewSource<TModel, TCell> dataSource;
        private UITableView tableView;

        public ReactiveTableViewBinder(UITableView tableView, ReactiveSectionedListTableViewSource<TModel, TCell> dataSource)
        {
            this.tableView = tableView;
            this.dataSource = dataSource;

            tableView.Source = dataSource;

            dataSource.CollectionChanges
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(handleCollectionChanges)
                .DisposedBy(disposeBag);
        }

        public void handleCollectionChanges(IReadOnlyCollection<ICollectionChange> changes)
        {
            lock (animationLock)
            {
                if (changes.Any(c => c is ReloadCollectionChange))
                {
                    tableView.ReloadData();
                    return;
                }

                tableView.BeginUpdates();

                foreach (var change in changes)
                {
                    var affectedSections = new List<int>();

                    switch (change)
                    {
                        case AddRowCollectionChange<TModel> addRow:
                            var addedToSection = add(addRow);
                            affectedSections.Add(addedToSection);
                            break;

                        case InsertSectionCollectionChange<TModel> insert:
                            insertSection(insert);
                            break;

                        case RemoveRowCollectionChange removeRow:
                            var removedFromSection = remove(removeRow);
                            affectedSections.Add(removedFromSection);
                            break;

                        case MoveRowWithinExistingSectionsCollectionChange<TModel> moveRow:
                            var oldAndNewSection = move(moveRow);
                            affectedSections.AddRange(oldAndNewSection);
                            break;

                        case MoveRowToNewSectionCollectionChange<TModel> moveRowToNewSection:
                            var oldSection = move(moveRowToNewSection);
                            affectedSections.Add(oldSection);
                            break;

                        case UpdateRowCollectionChange<TModel> updateRow:
                            var updatedInSection = update(updateRow);
                            affectedSections.Add(updatedInSection);
                            break;
                    }

                    dataSource.ChangeDisplayedCollection(change);

                    affectedSections
                        .Distinct()
                        .Where(index => index < dataSource.NumberOfSections(tableView))
                        .ForEach(index => dataSource.RefreshHeader(tableView, index));
                }

                tableView.EndUpdates();
            }
        }

        public void Dispose()
        {
            disposeBag?.Dispose();
            dataSource?.Dispose();
            tableView?.Dispose();
        }

        private void insertSection(InsertSectionCollectionChange<TModel> change)
        {
            tableView.InsertSections(indexSet(change.Index), UITableViewRowAnimation.Automatic);
            tableView.InsertRows(new SectionedIndex(change.Index, 0).ToIndexPaths(), UITableViewRowAnimation.Automatic);
        }

        private int add(AddRowCollectionChange<TModel> change)
        {
            tableView.InsertRows(change.Index.ToIndexPaths(), UITableViewRowAnimation.Automatic);
            return change.Index.Section;
        }

        private int remove(RemoveRowCollectionChange change)
        {
            remove(change.Index);
            return change.Index.Section;
        }

        private bool remove(SectionedIndex index)
        {
            if (dataSource.SectionContainsOnlyOneRow(index.Section))
            {
                tableView.DeleteSections(indexSet(index.Section), UITableViewRowAnimation.Automatic);
                return true;
            }

            tableView.DeleteRows(index.ToIndexPaths(), UITableViewRowAnimation.Automatic);
            return false;
        }

        private int move(MoveRowToNewSectionCollectionChange<TModel> change)
        {
            remove(change.OldIndex);
            tableView.InsertSections(indexSet(change.Index), UITableViewRowAnimation.Automatic);
            tableView.InsertRows(new SectionedIndex(change.Index, 0).ToIndexPaths(), UITableViewRowAnimation.Automatic);

            return change.OldIndex.Section;
        }

        private int[] move(MoveRowWithinExistingSectionsCollectionChange<TModel> change)
        {
            var oldSectionWasRemoved = remove(change.OldIndex);
            tableView.InsertRows(change.Index.ToIndexPaths(), UITableViewRowAnimation.Automatic);

            if (oldSectionWasRemoved)
            {
                // The index of the section which needs refreshing must be the index before the changes happen.
                // The index of the target section might change if the old section was in front of the target one
                // and the old one was removed. The original index of the target section would then be a one more
                // than the index after hte operation (which is the value of `change.Index.Section`).
                return new[] { change.Index.Section + change.OldIndex.Section <= change.Index.Section ? 1 : 0 };
            }

            return new[] { change.OldIndex.Section, change.Index.Section };
        }

        private int update(UpdateRowCollectionChange<TModel> change)
        {
            tableView.ReloadRows(change.Index.ToIndexPaths(), UITableViewRowAnimation.Automatic);
            return change.Index.Section;
        }

        private NSMutableIndexSet indexSet(int section)
            => (NSMutableIndexSet)NSIndexSet.FromIndex(section).MutableCopy();
    }
}
