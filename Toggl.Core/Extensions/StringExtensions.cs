using System.Collections.Generic;
using System.Linq;

namespace Toggl.Core.Extensions
{
    public static class StringExtensions
    {
        public static IList<string> SplitToQueryWords(this string text)
            => text.Split(' ')
                .Where(word => !string.IsNullOrEmpty(word))
                .Distinct()
                .ToList();
    }
}
