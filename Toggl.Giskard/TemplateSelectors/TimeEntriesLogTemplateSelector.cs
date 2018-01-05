using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;
using Toggl.Foundation.MvvmCross.Collections;

namespace Toggl.Giskard.TemplateSelectors
{
    public sealed class TimeEntriesLogTemplateSelector : IMvxTemplateSelector
    {
        private const int header = 0;
        private const int item = 1;

        public int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == header)
                return Resource.Layout.TimeEntriesLogFragmentHeader;

            return Resource.Layout.TimeEntriesLogFragmentCell;
        }

        public int GetItemViewType(object forItemObject)
        {
            if (forItemObject is TimeEntryViewModelCollection)
                return header;

            return item;
        }
    }
}
