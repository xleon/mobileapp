using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.WeakSubscription;
using MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.ViewHolders;

namespace Toggl.Giskard.Adapters
{
    public class MainRecyclerAdapter : ReactiveSectionedRecyclerAdapter<TimeEntryViewModel, MainLogCellViewHolder, MainLogSectionViewHolder>
    {
        public const int SuggestionViewType = 2;

        public IObservable<TimeEntryViewModel> TimeEntryTaps
            => timeEntryTappedSubject.AsObservable();

        public IObservable<TimeEntryViewModel> ContinueTimeEntrySubject
            => continueTimeEntrySubject.AsObservable();

        public IObservable<TimeEntryViewModel> DeleteTimeEntrySubject
            => deleteTimeEntrySubject.AsObservable();

        public SuggestionsViewModel SuggestionsViewModel { get; set; }

        private Subject<TimeEntryViewModel> timeEntryTappedSubject = new Subject<TimeEntryViewModel>();
        private Subject<TimeEntryViewModel> continueTimeEntrySubject = new Subject<TimeEntryViewModel>();
        private Subject<TimeEntryViewModel> deleteTimeEntrySubject = new Subject<TimeEntryViewModel>();

        public MainRecyclerAdapter(ObservableGroupedOrderedCollection<TimeEntryViewModel> items) : base(items)
        {
        }

        public void ContinueTimeEntry(int position)
        {
            var continuedTimeEntry = GetItemFromAdapterPosition(position);
            NotifyItemChanged(position);
            continueTimeEntrySubject.OnNext(continuedTimeEntry);
        }

        public void DeleteTimeEntry(int position)
        {
            var deletedTimeEntry = GetItemFromAdapterPosition(position);
            deleteTimeEntrySubject.OnNext(deletedTimeEntry);
        }

        public override int HeaderOffset => 1;

        protected override bool TryBindCustomViewType(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is MainLogSuggestionsListViewHolder suggestionsViewHolder)
            {
                suggestionsViewHolder.UpdateView();
                return true;
            }

            return false;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            if (viewType == SuggestionViewType)
            {
                var suggestionsView = LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.MainSuggestions, parent, false);
                return new MainLogSuggestionsListViewHolder(suggestionsView, SuggestionsViewModel);
            }

            return base.OnCreateViewHolder(parent, viewType);
        }

        public override int GetItemViewType(int position)
        {
            if (position == 0)
            {
                return SuggestionViewType;
            }

            return base.GetItemViewType(position);
        }

        protected override MainLogSectionViewHolder CreateHeaderViewHolder(ViewGroup parent)
        {
            return new MainLogSectionViewHolder(LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.MainLogHeader, parent, false));
        }

        protected override MainLogCellViewHolder CreateItemViewHolder(ViewGroup parent)
        {
            return new MainLogCellViewHolder(LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.MainLogCell, parent, false))
            {
                TappedSubject = timeEntryTappedSubject,
                ContinueButtonTappedSubject = continueTimeEntrySubject
            };
        }
    }
}
