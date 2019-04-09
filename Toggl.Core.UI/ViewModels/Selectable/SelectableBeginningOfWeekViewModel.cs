using System;
using MvvmCross.ViewModels;
using Toggl.Core.MvvmCross.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class SelectableBeginningOfWeekViewModel : IDiffableByIdentifier<SelectableBeginningOfWeekViewModel>
    {
        public BeginningOfWeek BeginningOfWeek { get; }

        public bool Selected { get; set; }

        public long Identifier => (long)BeginningOfWeek;

        public SelectableBeginningOfWeekViewModel(BeginningOfWeek beginningOfWeek, bool selected)
        {
            BeginningOfWeek = beginningOfWeek;
            Selected = selected;
        }

        public bool Equals(SelectableBeginningOfWeekViewModel other) 
            => BeginningOfWeek == other.BeginningOfWeek && Selected == other.Selected;
    }
}
