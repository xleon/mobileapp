using System.Collections.Generic;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    public struct SelectableItem<TValue>
    {
        public string Title { get; set; }

        public TValue Value { get; set; }
    }

    public struct SelectFromListParameters<TValue>
    {
        public IList<SelectableItem<TValue>> Items { get; set; }

        public int SelectedIndex { get; set; }
    }
}
