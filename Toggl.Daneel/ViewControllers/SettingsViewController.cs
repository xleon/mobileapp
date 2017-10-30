﻿using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Converters;
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

            var inverseBoolConverter = new BoolToConstantValueConverter<bool>(false, true);

            var bindingSet = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();

            bindingSet.Bind(LogoutButton).To(vm => vm.LogoutCommand);
            bindingSet.Bind(LogoutButton)
                      .For(btn => btn.Enabled)
                      .To(vm => vm.IsLoggingOut)
                      .WithConversion(inverseBoolConverter);

            bindingSet.Bind(NavigationItem)
                      .For(nav => nav.BindHidesBackButton())
                      .To(vm => vm.IsLoggingOut);

            bindingSet.Apply();

            LogoutButton.SetTitleColor(Color.Settings.SignOutButtonDisabled.ToNativeColor(), UIKit.UIControlState.Disabled);
        }
    }
}
