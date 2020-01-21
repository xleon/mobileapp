using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Toggl.Core.UI.ViewModels.Reports
{
    public interface IReportElement : IEquatable<IReportElement>
    {
    }

    public static class IReportElementExtensions
    {
        public static ImmutableList<IReportElement> Flatten(this IEnumerable<IReportElement> elements)
        {
            return elements.SelectMany(e => e.flatten()).ToImmutableList();
        }

        private static IEnumerable<IReportElement> flatten(this IReportElement element)
        {
            if (element is CompositeReportElement composite)
            {
                var subElements = composite.SubElements;
                foreach (var item in subElements.SelectMany(e => e.flatten()))
                    yield return item;

                yield break;
            }

            yield return element;
        }
    }
}
