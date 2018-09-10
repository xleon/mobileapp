using System;
using System.Windows.Input;
using Android.Runtime;
using Android.Views;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Toggl.Giskard.ViewHolders
{
    public class GuardedMvxRecyclerViewHolder : MvxRecyclerViewHolder
    {
        public Func<object, bool> CanExecute { get; set; } = obj => true;

        public GuardedMvxRecyclerViewHolder(View itemView, IMvxAndroidBindingContext context) : base(itemView, context)
        {
        }

        public GuardedMvxRecyclerViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void ExecuteCommandOnItem(ICommand command)
        {
            if (CanExecute(DataContext))
            {
                base.ExecuteCommandOnItem(command);
            }
        }
    }
}
