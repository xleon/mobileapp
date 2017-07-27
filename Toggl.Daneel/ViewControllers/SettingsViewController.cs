﻿using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public partial class SettingsViewController : MvxViewController<SettingsViewModel>
    {
        public SettingsViewController() 
            : base(nameof(SettingsViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = ViewModel.Title;

            var bindingSet = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();
            bindingSet.Bind(LogoutButton).To(vm => vm.LogoutCommand);
            bindingSet.Apply();
        }
    }
}
