using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Suggestions;
using Toggl.Giskard.Adapters;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.ViewHelpers;
using Toggl.Multivac.Extensions;
using TogglResources = Toggl.Foundation.Resources;

namespace Toggl.Giskard.ViewHolders
{
    public class MainLogSuggestionsListViewHolder : RecyclerView.ViewHolder, IReactiveBindingHolder
    {
        private SuggestionsViewModel suggestionsViewModel;

        private TextView hintTextView;
        private TextView indicatorTextView;
        private RecyclerView suggestionsRecyclerView;
        private MainSuggestionsRecyclerAdapter mainSuggestionsRecyclerAdapter;

        public CompositeDisposable DisposeBag { get; } = new CompositeDisposable();

        private int currentSuggestionCard = 1;

        public MainLogSuggestionsListViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public MainLogSuggestionsListViewHolder(View itemView, SuggestionsViewModel suggestionsViewModel) : base(itemView)
        {
            this.suggestionsViewModel = suggestionsViewModel;

            hintTextView = ItemView.FindViewById<TextView>(Resource.Id.SuggestionsHintTextView);
            indicatorTextView = ItemView.FindViewById<TextView>(Resource.Id.SuggestionsIndicatorTextView);
            suggestionsRecyclerView = ItemView.FindViewById<RecyclerView>(Resource.Id.SuggestionsRecyclerView);

            suggestionsRecyclerView.SetLayoutManager(new LinearLayoutManager(ItemView.Context, LinearLayoutManager.Horizontal, false));
            var snapMargin = 16.DpToPixels(ItemView.Context);
            var snapHelper = new SuggestionsRecyclerViewSnapHelper(snapMargin);
            snapHelper.AttachToRecyclerView(suggestionsRecyclerView);

            this.Bind(snapHelper.CurrentIndexObservable, onCurrentSuggestionIndexChanged);

            mainSuggestionsRecyclerAdapter = new MainSuggestionsRecyclerAdapter();
            this.Bind(mainSuggestionsRecyclerAdapter.SuggestionTaps, onSuggestionTapped);

            suggestionsRecyclerView.SetAdapter(mainSuggestionsRecyclerAdapter);

            var collectionChangesObservable = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                e => this.suggestionsViewModel.Suggestions.CollectionChanged += e,
                e => this.suggestionsViewModel.Suggestions.CollectionChanged -= e);

            var collectionChangedObservable = collectionChangesObservable
                .Select(args => this.suggestionsViewModel.Suggestions.ToImmutableList())
                .StartWith(this.suggestionsViewModel.Suggestions.ToImmutableList());

            var suggestionsCountObservable = collectionChangesObservable
                .Select(args => this.suggestionsViewModel.Suggestions.Count)
                .StartWith(this.suggestionsViewModel.Suggestions.Count)
                .DistinctUntilChanged();

            this.Bind(collectionChangedObservable, onSuggestionsCollectionChanged);
            this.Bind(suggestionsCountObservable, onCollectionCountChanged);
        }

        public void UpdateView()
        {
            if (suggestionsViewModel.Suggestions.None())
            {
                hintTextView.Visibility = ViewStates.Gone;
                indicatorTextView.Visibility = ViewStates.Gone;
                suggestionsRecyclerView.Visibility = ViewStates.Gone;
            }
            else
            {
                hintTextView.Visibility = ViewStates.Visible;
                suggestionsRecyclerView.Visibility = ViewStates.Visible;
                updateHintText();
            }
        }

        private void onSuggestionTapped(Suggestion suggestion)
        {
            if (suggestionsViewModel == null) return;
            if (suggestionsViewModel.StartTimeEntryCommand.CanExecute())
            {
                suggestionsViewModel.StartTimeEntryCommand.Execute(suggestion);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;

            DisposeBag.Dispose();
        }

        private void onSuggestionsCollectionChanged(ImmutableList<Suggestion> suggestions)
        {
            mainSuggestionsRecyclerAdapter.UpdateDataset(suggestions);
            UpdateView();
        }

        private void onCurrentSuggestionIndexChanged(int currentIndex)
        {
            currentSuggestionCard = currentIndex;
            updateHintText();
        }

        private void onCollectionCountChanged(int itemCount)
        {
            updateHintText();
        }

        private void updateHintText()
        {
            var numberOfSuggestions = suggestionsViewModel.Suggestions.Count;

            switch (numberOfSuggestions)
            {
                case 0:
                    return;

                case 1:
                    hintTextView.Text = TogglResources.WorkingOnThis;
                    indicatorTextView.Visibility = ViewStates.Gone;
                    break;

                default:
                    var indicatorText = $"{currentSuggestionCard} {TogglResources.Of.ToUpper()} {numberOfSuggestions}";
                    hintTextView.Text = TogglResources.WorkingOnThese;
                    indicatorTextView.Visibility = ViewStates.Visible;
                    indicatorTextView.Text = indicatorText;
                    break;
            }

        }
    }
}
