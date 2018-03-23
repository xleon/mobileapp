using MvvmCross.Droid.Support.V7.RecyclerView.ItemTemplates;

namespace Toggl.Giskard.TemplateSelectors
{
    public sealed class ReportsTemplateSelector : IMvxTemplateSelector
    {
        public const int Header = 0;
        public const int Detail = 1;

        public int GetItemLayoutId(int fromViewType)
            => Resource.Layout.ReportsActivityHeader;

        public int GetItemViewType(object forItemObject)
            => Header;
    }
}
