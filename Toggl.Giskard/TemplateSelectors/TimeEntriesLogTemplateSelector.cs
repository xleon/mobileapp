using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;
using Toggl.Foundation.MvvmCross.Collections;

namespace Toggl.Giskard.TemplateSelectors
{
    public sealed class TimeEntriesLogTemplateSelector : IMvxTemplateSelector
    {
        private const int header = 0;
        private const int item = 1;
        private const int footer = 2;

        public int GetItemLayoutId(int fromViewType)
        {
            if (fromViewType == header)
                return Resource.Layout.TimeEntriesLogFragmentHeader;

            if (fromViewType == footer)
                return Resource.Layout.TimeEntriesLogFragmentFooter;

            return Resource.Layout.TimeEntriesLogFragmentCell;
        }

        public int GetItemViewType(object forItemObject)
        {
            if (forItemObject == null)
                return footer;

            if (forItemObject is TimeEntryViewModelCollection)
                return header;

            return item;
        }
    }
}
