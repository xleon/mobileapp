﻿using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Plugins.Color.iOS;
using MvvmCross.Plugins.Visibility;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

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

            prepareViews();

            Title = ViewModel.Title;

            var inverseBoolConverter = new BoolToConstantValueConverter<bool>(false, true);
            var visibilityConverter = new MvxVisibilityValueConverter();

            var bindingSet = this.CreateBindingSet<SettingsViewController, SettingsViewModel>();

            bindingSet.Bind(LogoutButton).To(vm => vm.LogoutCommand);
            bindingSet.Bind(LogoutButton)
                      .For(btn => btn.Enabled)
                      .To(vm => vm.IsLoggingOut)
                      .WithConversion(inverseBoolConverter);

            bindingSet.Bind(NavigationItem)
                      .For(nav => nav.BindHidesBackButton())
                      .To(vm => vm.IsLoggingOut);

            bindingSet.Bind(SyncingView)
                      .For(view => view.BindVisibility())
                      .To(vm => vm.IsRunningSync)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(SyncedView)
                      .For(view => view.BindVisibility())
                      .To(vm => vm.IsSynced)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(LoggingOutView)
                      .For(view => view.BindVisibility())
                      .To(vm => vm.IsLoggingOut)
                      .WithConversion(visibilityConverter);

            bindingSet.Apply();
        }

        private void prepareViews()
        {
            LogoutButton.SetTitleColor(Color.Settings.SignOutButtonDisabled.ToNativeColor(), UIKit.UIControlState.Disabled);

            setIndicatorSyncColor(SyncingIndicator);
            setStatusTextColor(SyncingLabel);

            setIndicatorSyncColor(SyncedIcon);
            setStatusTextColor(SyncedLabel);

            setIndicatorSyncColor(LoggingOutIndicator);
            setStatusTextColor(LoggingOutLabel);
        }

        private void setIndicatorSyncColor(UIImageView imageView)
        {
            imageView.Image = imageView.Image.ImageWithRenderingMode(UIKit.UIImageRenderingMode.AlwaysTemplate);
            imageView.TintColor = Color.Settings.SyncStatusText.ToNativeColor();
        }

        private void setStatusTextColor(UILabel label)
        {
            label.TextColor = Color.Settings.SyncStatusText.ToNativeColor();
        }
    }
}
