namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public struct InsertSectionCollectionChange<T> : ICollectionChange
    {
        public int Index { get; }

        public T Item { get; }

        public InsertSectionCollectionChange(int index, T item)
        {
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Insert Section: {Index} ({Item})";
    }
}
