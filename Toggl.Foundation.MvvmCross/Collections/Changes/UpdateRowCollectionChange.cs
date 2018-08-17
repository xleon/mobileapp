namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public struct UpdateRowCollectionChange<T> : ICollectionChange
    {
        public SectionedIndex Index { get; }

        public T Item { get; }

        public UpdateRowCollectionChange(SectionedIndex index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Update row: {Index} ({Item})";
    }
}
