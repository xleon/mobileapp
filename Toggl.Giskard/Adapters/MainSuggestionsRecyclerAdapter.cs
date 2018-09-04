using System;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross;
using MvvmCross.Platforms.Android;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.Suggestions;
using Toggl.Giskard.Extensions;
using Toggl.Giskard.ViewHolders;
using Toggl.Giskard.Views;

namespace Toggl.Giskard.Adapters
{
    public class MainSuggestionsRecyclerAdapter : RecyclerView.Adapter
    {
        public IObservable<Suggestion> SuggestionTaps
            => suggestionTappedSubject.AsObservable();

        private static readonly int defaultCardSize;
        private static readonly int firstItemMargin;
        private static readonly int lastItemMargin;

        private ImmutableList<Suggestion> currentSuggestions = ImmutableList<Suggestion>.Empty;

        private readonly Subject<Suggestion> suggestionTappedSubject = new Subject<Suggestion>();

        static MainSuggestionsRecyclerAdapter()
        {
            var context = Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext;
            var service = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var display = service.DefaultDisplay;
            var size = new Point();
            display.GetSize(size);
            firstItemMargin = 14.DpToPixels(context);
            defaultCardSize = 200.DpToPixels(context);
            lastItemMargin = size.X - defaultCardSize - firstItemMargin;
        }

        public void UpdateDataset(ImmutableList<Suggestion> suggestions)
        {
            currentSuggestions = suggestions;
            NotifyDataSetChanged();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (position >= currentSuggestions.Count) return;

            var suggestionsViewHolder = (MainLogSuggestionItemViewHolder) holder;
            suggestionsViewHolder.Item = currentSuggestions[position];

            suggestionsViewHolder.IsFirstItem = position == 0;
            suggestionsViewHolder.IsLastItem = position == ItemCount - 1;
            suggestionsViewHolder.IsSingleItem = ItemCount == 1;
            suggestionsViewHolder.RecalculateMargins();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.MainSuggestionsCard, parent, false);
            return new MainLogSuggestionItemViewHolder(view, firstItemMargin, lastItemMargin, defaultCardSize)
            {
                TappedSubject = suggestionTappedSubject
            };
        }

        public override int ItemCount => currentSuggestions.Count;
    }
}
