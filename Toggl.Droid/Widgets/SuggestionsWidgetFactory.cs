using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Shared;
using Color = Android.Graphics.Color;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Widgets
{
    public sealed class SuggestionsWidgetFactory : Java.Lang.Object, RemoteViewsService.IRemoteViewsFactory
    {
        private Context context;
        private List<WidgetSuggestionItem> items = new List<WidgetSuggestionItem>();

        public SuggestionsWidgetFactory(Context context)
        {
            this.context = context;
        }

        public void OnCreate()
        {
            updateData();
        }

        public void OnDataSetChanged()
        {
            updateData();
        }

        public int Count
            => items?.Count ?? 0;

        public bool HasStableIds
            => false;

        public RemoteViews GetViewAt(int position)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.SuggestionsWidgetItem);

            var item = items[position];
            var hasDescription = !string.IsNullOrEmpty(item.Description);
            var hasProject = !string.IsNullOrEmpty(item.ProjectName);

            view.SetViewVisibility(Resource.Id.DescriptionTextView, hasDescription.ToVisibility());
            if (hasDescription)
                view.SetTextViewText(Resource.Id.DescriptionTextView, item.Description);

            view.SetViewVisibility(Resource.Id.ProjectClientRow, hasProject.ToVisibility());

            if (hasProject)
            {
                view.SetTextViewText(Resource.Id.ProjectNameTextView, item.ProjectName);
                view.SetTextColor(Resource.Id.ProjectNameTextView, Color.ParseColor(item.ProjectColor));
                view.SetTextViewText(Resource.Id.ClientNameTextView, item.ClientName);
            }

            var bottomBorderVisibility = (position != Count - 1).ToVisibility();
            view.SetViewVisibility(Resource.Id.BottomSeparator, bottomBorderVisibility);

            return view;
        }

        // We are using a default loading view
        public RemoteViews LoadingView => null;

        // This requires only a single view type, so we're using number 1 to reference that type.
        public int ViewTypeCount => 1;

        public long GetItemId(int position) => position;

        private void updateData()
        {
            items.Clear();
            items.AddRange(WidgetSuggestionItem.SuggestionsFromSharedPreferences());
        }

        public void OnDestroy()
        {
            items.Clear();
        }

        public static RemoteViews Setup(Context context, int widgetId)
        {
            var view = new RemoteViews(context.PackageName, Resource.Layout.SuggestionsWidget);

            var intent = new Intent(context, JavaUtils.ToClass<SuggestionsWidgetService>());
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetId, widgetId);

            view.SetRemoteAdapter(Resource.Id.SuggestionsList, intent);
            view.SetEmptyView(Resource.Id.SuggestionsList, Resource.Id.NoData);

            view.SetTextViewText(Resource.Id.Title, Resources.WorkingOnThese);
            view.SetTextViewText(Resource.Id.ShowAllTimeEntriesLabel, Resources.ShowAllTimEntries);
            view.SetTextViewText(Resource.Id.NoData, Resources.NoSuggestionsAvailable);

            return view;
        }
    }
}
