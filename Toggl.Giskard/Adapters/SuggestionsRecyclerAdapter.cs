using System;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Toggl.Giskard.Views;

namespace Toggl.Giskard.Adapters
{
    public sealed class SuggestionsRecyclerAdapter : MvxRecyclerAdapter
    {
        public SuggestionsRecyclerAdapter()
        {
        }

        public SuggestionsRecyclerAdapter(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemBindingContext = new MvxAndroidBindingContext(parent.Context, BindingContext.LayoutInflaterHolder);
            var inflatedView = InflateViewForHolder(parent, viewType, itemBindingContext);
            var viewHolder = new SuggestionsRecyclerViewHolder(inflatedView, itemBindingContext)
            {
                Click = ItemClick,
                LongClick = ItemLongClick
            };

            return viewHolder;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            base.OnBindViewHolder(holder, position);

            var suggestionsViewHolder = (SuggestionsRecyclerViewHolder)holder;
            suggestionsViewHolder.IsFirstItem = position == 0;
            suggestionsViewHolder.IsLastItem = position == ItemCount - 1;
            suggestionsViewHolder.IsSingleItem = ItemCount == 1;
            suggestionsViewHolder.RecalculateMargins();
        }
    }
}
