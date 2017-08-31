using MvvmCross.Core.ViewModels;

namespace Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions
{
    public abstract class BaseTimeEntrySuggestionViewModel : MvxNotifyPropertyChanged 
    {
        public abstract override int GetHashCode();
    }
}
