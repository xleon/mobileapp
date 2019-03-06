using System;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;

namespace Toggl.Giskard.Views.Calendar
{
    public partial class CalendarLayoutManager
    {
        private int computeScrollOffset(RecyclerView.State state, View startChild, View endChild)
        {
            if (cannotCalculateScrollVariables(state, startChild, endChild))
                return 0;

            var startChildPosition = GetPosition(startChild);
            var anchorHeight = (double)Math.Abs(orientationHelper.GetDecoratedEnd(startChild) - orientationHelper.GetDecoratedStart(startChild));

            return (int)Math.Round(startChildPosition * anchorHeight +
                                    (orientationHelper.StartAfterPadding - orientationHelper.GetDecoratedStart(startChild)));
        }

        private int computeScrollExtent(RecyclerView.State state, View startChild, View endChild)
        {
            if (cannotCalculateScrollVariables(state, startChild, endChild))
                return 0;

            var extent = orientationHelper.GetDecoratedEnd(endChild) - orientationHelper.GetDecoratedStart(startChild);
            return Math.Min(orientationHelper.TotalSpace, extent);
        }

        private int computeScrollRange(RecyclerView.State state, View startChild, View endChild)
        {
            if (cannotCalculateScrollVariables(state, startChild, endChild))
                return 0;

            var laidOutArea = orientationHelper.GetDecoratedEnd(endChild) - orientationHelper.GetDecoratedStart(startChild);
            var laidOutRangeItemCount = Math.Abs(GetPosition(startChild) - GetPosition(endChild)) + 1;

            //todo: simplify to use only the anchor height * anchor count
            return (int)((float)laidOutArea / laidOutRangeItemCount * anchorCount);
        }

        private bool cannotCalculateScrollVariables(RecyclerView.State state, View startChild, View endChild)
            => ChildCount == 0 || state.ItemCount == 0 || startChild == null || endChild == null;
    }
}
