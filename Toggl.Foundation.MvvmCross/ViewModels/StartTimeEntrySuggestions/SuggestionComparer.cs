using System.Collections.Generic;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public sealed class SuggestionComparer : IEqualityComparer<BaseTimeEntrySuggestionViewModel>
    {
        public static SuggestionComparer Instance { get; } = new SuggestionComparer();

        private SuggestionComparer() { }

        public bool Equals(BaseTimeEntrySuggestionViewModel x, BaseTimeEntrySuggestionViewModel y)
        {
            switch (x)
            {
                case TimeEntrySuggestionViewModel teX:
                    return y is TimeEntrySuggestionViewModel teY
                        && teX.Description == teY.Description && teX.ProjectId == teY.ProjectId;

                case ProjectSuggestionViewModel pX:
                    return y is ProjectSuggestionViewModel pY && pX.ProjectId == pY.ProjectId;
            }

            return x == y;
        }

        public int GetHashCode(BaseTimeEntrySuggestionViewModel obj)
            => obj.GetHashCode();
    }
}
