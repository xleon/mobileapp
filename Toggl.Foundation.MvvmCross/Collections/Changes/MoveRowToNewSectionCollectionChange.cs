namespace Toggl.Foundation.MvvmCross.Collections.Changes
{
    public struct MoveRowToNewSectionCollectionChange<T> : ICollectionChange
    {
        public SectionedIndex OldIndex { get; }

        public int Index { get; }

        public T Item { get; }

        public MoveRowToNewSectionCollectionChange(SectionedIndex oldIndex, int index, T item)
        {
            OldIndex = oldIndex;
            Index = index;
            Item = item;
        }

        public override string ToString() => $"Move to new section: {OldIndex} -> {Index} ({Item})";
    }
}
