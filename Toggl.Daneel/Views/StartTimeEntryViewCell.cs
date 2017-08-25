using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Plugins.Color;
using MvvmCross.Plugins.Visibility;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.StartTimeEntrySuggestions;
using UIKit;

namespace Toggl.Daneel.Views
{
    public partial class StartTimeEntryViewCell : MvxTableViewCell
    {
        private const float NoProjectDistance = 16;
        private const float HasProjectDistance = 8;

        public static readonly NSString Key = new NSString(nameof(StartTimeEntryViewCell));
        public static readonly UINib Nib;

        static StartTimeEntryViewCell()
        {
            Nib = UINib.FromName(nameof(StartTimeEntryViewCell), NSBundle.MainBundle);
        }

        protected StartTimeEntryViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.DelayBind(() =>
            {
                var colorConverter = new MvxRGBValueConverter();
                var visibilityConverter = new MvxVisibilityValueConverter();
                var timeSpanConverter = new TimeSpanToDurationValueConverter();
                var descriptionTopDistanceValueConverter = new BoolToConstantValueConverter<nfloat>(HasProjectDistance, NoProjectDistance);

                var bindingSet = this.CreateBindingSet<StartTimeEntryViewCell, TimeEntrySuggestionViewModel>();

                //Text
                bindingSet.Bind(ProjectLabel).To(vm => vm.ProjectName);
                bindingSet.Bind(DescriptionLabel).To(vm => vm.Description);

                //Color
                bindingSet.Bind(ProjectLabel)
                          .For(v => v.TextColor)
                          .To(vm => vm.ProjectColor)
                          .WithConversion(colorConverter);

                bindingSet.Bind(ProjectDotView)
                          .For(v => v.BackgroundColor)
                          .To(vm => vm.ProjectColor)
                          .WithConversion(colorConverter);

                //Visibility
                bindingSet.Bind(DescriptionTopDistanceConstraint)
                          .For(v => v.Constant)
                          .To(vm => vm.HasProject)
                          .WithConversion(descriptionTopDistanceValueConverter);

                bindingSet.Bind(ProjectLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Bind(ProjectDotView)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Apply();
            });
        }
    }
}
