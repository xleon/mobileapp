using Android.Support.V7.Widget;
using Android.Views;

namespace Toggl.Giskard.Views
{     public sealed class SuggestionsRecyclerViewSnapHelper : LinearSnapHelper
    {
        private OrientationHelper horizontalHelper;

        public override View FindSnapView(RecyclerView.LayoutManager layoutManager)
            => getStartView(layoutManager, getHorizontalHelper(layoutManager));

        public override int[] CalculateDistanceToFinalSnap(RecyclerView.LayoutManager layoutManager, View targetView)
            => new[] { distanceToStart(targetView, getHorizontalHelper(layoutManager)), 0 };

        private int distanceToStart(View targetView, OrientationHelper helper)
            => helper.GetDecoratedStart(targetView) - helper.StartAfterPadding;

        private OrientationHelper getHorizontalHelper(RecyclerView.LayoutManager layoutManager)
            => horizontalHelper ?? (horizontalHelper = OrientationHelper.CreateHorizontalHelper(layoutManager));

        private View getStartView(RecyclerView.LayoutManager layoutManager, OrientationHelper helper)
        {
            var linearLayoutManager = (LinearLayoutManager)layoutManager;

            var firstChildIndex = linearLayoutManager.FindFirstVisibleItemPosition();
            if (firstChildIndex == RecyclerView.NoPosition) 
                return null;

            var lastItemIndex = linearLayoutManager.ItemCount - 1;
            var isLastItem = linearLayoutManager.FindLastCompletelyVisibleItemPosition() == lastItemIndex;
            if (isLastItem) 
                return null;

            var firstView = layoutManager.FindViewByPosition(firstChildIndex);
            var decoratedEnd = helper.GetDecoratedEnd(firstView);
            var threshold = helper.GetDecoratedMeasurement(firstView) / 2;
            if (decoratedEnd >= threshold && decoratedEnd > 0)
                return firstView;

            return layoutManager.FindViewByPosition(firstChildIndex + 1);
        }
    }
}
