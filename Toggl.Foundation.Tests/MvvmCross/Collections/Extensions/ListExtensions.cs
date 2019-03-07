using System;
using System.Collections.Generic;
using System.Linq;
using Toggl.Foundation.MvvmCross.Collections;
using Toggl.Foundation.MvvmCross.Collections.Diffing;

namespace Toggl.Foundation.Tests.MvvmCross.Collections.Extensions
{
    public static class ListExtensions
    {
        public static List<TSection> Apply<TSection, THeader, TElement>(this List<TSection> list,
            List<Diffing<TSection, THeader, TElement>.Changeset> changes)
        where TSection : IAnimatableSectionModel<THeader, TElement>, new()
        where THeader : IDiffable
        where TElement : IDiffable, IEquatable<TElement>
        {
            return changes.Aggregate(list, (sections, changeset) =>
            {
                var newSections = changeset.Apply(original: sections);
                return newSections;
            });
        }
    }
}
