using Android.Views;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Droid.ViewHolders.MainLog;
using Android.Runtime;
using Android.Support.V7.Widget;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Interfaces;
using Toggl.Core.UI.ViewModels.MainLog.Identity;
using Toggl.Droid.Adapters.DiffingStrategies;
using Toggl.Droid.ViewHolders;
using Toggl.Shared;

namespace Toggl.Droid.Adapters
{
    using MainLogSection = AnimatableSectionModel<MainLogSectionViewModel, MainLogItemViewModel, IMainLogKey>;

    public class MainLogRecyclerAdapter : BaseRecyclerAdapter<MainLogItemViewModel>
    {
        public const int TimeEntryLogItemViewType = 1;
        public const int SuggestionLogItemViewType = 2;
        public const int DaySummaryViewType = 3;
        public const int SuggestionsHeaderViewType = 4;
        public const int UserFeedbackViewType = 5;

        // The following list includes the types of section headers that should not be rendered.
        // They will not be added to the list of items the adapter holds.
        private readonly ImmutableList<Type> nonRenderableHeaderTypes = new List<Type>()
        {
            typeof(UserFeedbackSectionViewModel)
        }.ToImmutableList();

        public IObservable<TimeEntryLogItemViewModel> EditTimeEntry
            => editTimeEntrySubject.AsObservable();

        public IObservable<GroupId> ToggleGroupExpansion
            => toggleGroupExpansionSubject.AsObservable();

        public IObservable<ContinueTimeEntryInfo> ContinueTimeEntry
            => continueTimeEntrySubject.AsObservable();

        public IObservable<TimeEntryLogItemViewModel> DeleteTimeEntrySubject
            => deleteTimeEntrySubject.AsObservable();

        public IObservable<SuggestionLogItemViewModel> ContinueSuggestion
            => continueSuggestionSubject.AsObservable();

        private readonly Subject<TimeEntryLogItemViewModel> editTimeEntrySubject = new Subject<TimeEntryLogItemViewModel>();
        private readonly Subject<GroupId> toggleGroupExpansionSubject = new Subject<GroupId>();
        private readonly Subject<TimeEntryLogItemViewModel> deleteTimeEntrySubject = new Subject<TimeEntryLogItemViewModel>();
        private readonly Subject<ContinueTimeEntryInfo> continueTimeEntrySubject = new Subject<ContinueTimeEntryInfo>();
        private readonly Subject<SuggestionLogItemViewModel> continueSuggestionSubject = new Subject<SuggestionLogItemViewModel>();

        public MainLogRecyclerAdapter(): base(new MainLogDiffingStrategy())
        {
        }

        public MainLogRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override int GetItemViewType(int position)
        {
            var item = GetItem(position);
            switch (item)
            {
                case TimeEntryLogItemViewModel _:
                    return TimeEntryLogItemViewType;
                case SuggestionLogItemViewModel _:
                    return SuggestionLogItemViewType;
                case DaySummaryViewModel _:
                    return DaySummaryViewType;
                case SuggestionsHeaderViewModel _:
                    return SuggestionsHeaderViewType;
                case UserFeedbackViewModel _:
                    return UserFeedbackViewType;
                default:
                    throw new Exception($"Invalid item type {item.GetSafeTypeName()}");
            }
        }

        protected override BaseRecyclerViewHolder<MainLogItemViewModel> CreateViewHolder(ViewGroup parent,
            LayoutInflater inflater, int viewType)
        {
            switch (viewType)
            {
                case TimeEntryLogItemViewType:
                    var logItemView = LayoutInflater.FromContext(parent.Context)
                        .Inflate(Resource.Layout.MainLogCell, parent, false);
                    return new MainLogCellViewHolder(logItemView)
                    {
                        EditTimeEntrySubject = editTimeEntrySubject,
                        ContinueButtonTappedSubject = continueTimeEntrySubject,
                        ToggleGroupExpansionSubject = toggleGroupExpansionSubject
                    };
                case SuggestionLogItemViewType:
                    var suggestionsView = LayoutInflater.FromContext(parent.Context)
                        .Inflate(Resource.Layout.MainSuggestionsCard, parent, false);
                    return new MainLogSuggestionItemViewHolder(suggestionsView)
                    {
                        ContinueSuggestionSubject = continueSuggestionSubject
                    };
                case DaySummaryViewType:
                    var sectionView = LayoutInflater.FromContext(parent.Context)
                        .Inflate(Resource.Layout.MainLogHeader, parent, false);
                    return new MainLogSectionViewHolder(sectionView);
                case SuggestionsHeaderViewType:
                    var suggestionsSectionView = LayoutInflater.FromContext(parent.Context)
                        .Inflate(Resource.Layout.MainLogSuggestionsHeader, parent, false);
                    return new MainLogSuggestionSectionViewHolder(suggestionsSectionView);
                case UserFeedbackViewType:
                    var userFeedbackView = LayoutInflater.FromContext(parent.Context)
                        .Inflate(Resource.Layout.MainUserFeedbackCard, parent, false);
                    return new MainLogUserFeedbackViewHolder(userFeedbackView);
                default:
                    throw new Exception($"Invalid view type: {viewType}");
            }
        }

        public void UpdateCollection(IImmutableList<MainLogSection> items)
        {
            var flattenItems = items.Aggregate(ImmutableList<MainLogItemViewModel>.Empty,
                (acc, nextSection) =>
                    nonRenderableHeaderTypes.Contains(nextSection.Header.GetType())
                        ? acc.AddRange(nextSection.Items)
                        : acc.AddRange(nextSection.Items.Prepend(nextSection.Header))
            );
            SetItems(flattenItems);
        }

        public void ContinueTimeEntryBySwiping(int position)
        {
            var continuedTimeEntry = GetItem(position) as TimeEntryLogItemViewModel;
            NotifyItemChanged(position);

            var continueMode = continuedTimeEntry.IsTimeEntryGroupHeader
                ? ContinueTimeEntryMode.TimeEntriesGroupSwipe
                : ContinueTimeEntryMode.SingleTimeEntrySwipe;

            continueTimeEntrySubject.OnNext(new ContinueTimeEntryInfo(continuedTimeEntry, continueMode));
        }

        public void DeleteTimeEntry(int position)
        {
            var deletedTimeEntry = GetItem(position) as TimeEntryLogItemViewModel;
            deleteTimeEntrySubject.OnNext(deletedTimeEntry);
        }

        private sealed class MainLogDiffingStrategy: IDiffingStrategy<MainLogItemViewModel>
        {
            public MainLogDiffingStrategy()
            {
                Ensure.Argument.TypeImplementsOrInheritsFromType(
                    derivedType: typeof(MainLogItemViewModel),
                    baseType: typeof(IDiffableByIdentifier<MainLogItemViewModel>));
            }

            public bool AreContentsTheSame(MainLogItemViewModel item, MainLogItemViewModel other)
            {
                return item.Equals(other);
            }

            public bool AreItemsTheSame(MainLogItemViewModel item, MainLogItemViewModel other)
            {
                var itemDiffable = (IDiffable<IMainLogKey>) item;
                var otherDiffable = (IDiffable<IMainLogKey>) other;

                // discard non matching types
                // comparing identities to distinguish group/non-group TimeEntryLogItemViewModel
                if (itemDiffable.Identity.GetType() != otherDiffable.Identity.GetType())
                {
                    return false;
                }

                if (item is TimeEntryLogItemViewModel timeEntry && other is TimeEntryLogItemViewModel otherTimeEntry)
                {
                    return timeEntry.Identity.Equals(otherTimeEntry.Identity);
                }

                return item.Identifier == other.Identifier;
            }

            public long GetItemId(MainLogItemViewModel item) => RecyclerView.NoId;

            public bool HasStableIds => false;
        }
    }
}
