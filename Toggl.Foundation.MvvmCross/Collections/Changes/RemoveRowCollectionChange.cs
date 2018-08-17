namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public struct RemoveRowCollectionChange : ICollectionChange
    {
        public SectionedIndex Index { get; }

        public RemoveRowCollectionChange(SectionedIndex index)
        {
            Index = index;
        }

        public override string ToString() => $"Remove row: {Index}";
    }
}
