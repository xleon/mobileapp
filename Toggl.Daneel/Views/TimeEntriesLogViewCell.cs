using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using MvvmCross.Plugins.Visibility;
using Toggl.Foundation.MvvmCross.ViewModels;
using MvvmCross.Binding.iOS;
using UIKit;
using Toggl.Daneel.Extensions;
using Toggl.Foundation.MvvmCross.Converters;
using MvvmCross.Plugins.Color;
using MvvmCross.Platform.Converters;
using System.Globalization;

namespace Toggl.Daneel.Views
{
    public partial class TimeEntriesLogViewCell : MvxTableViewCell
    {
        private class DescriptionTopDistanceValueConverter : MvxValueConverter<bool, nfloat>
        {
            private const float NoProjectDistance = 24;
            private const float HasProjectDistance = 14;

            protected override nfloat Convert(bool value, Type targetType, object parameter, CultureInfo culture)
                => value ? HasProjectDistance : NoProjectDistance;
        }

        public static readonly NSString Key = new NSString(nameof(TimeEntriesLogViewCell));
        public static readonly UINib Nib;

        static TimeEntriesLogViewCell()
        {
            Nib = UINib.FromName(nameof(TimeEntriesLogViewCell), NSBundle.MainBundle);
        }

        protected TimeEntriesLogViewCell(IntPtr handle) 
            : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            TimeLabel.Font = TimeLabel.Font.GetMonospacedDigitFont();

            this.DelayBind(() =>
            {
                var colorConverter = new MvxRGBValueConverter();
                var visibilityConverter = new MvxVisibilityValueConverter();
                var timeSpanConverter = new TimeSpanToDurationValueConverter();
                var descriptionTopDistanceValueConverter = new DescriptionTopDistanceValueConverter();
                
                var bindingSet = this.CreateBindingSet<TimeEntriesLogViewCell, TimeEntryViewModel>();

                //Text
                bindingSet.Bind(ProjectLabel).To(vm => vm.ProjectName);
                bindingSet.Bind(DescriptionLabel).To(vm => vm.Description);
                bindingSet.Bind(TimeLabel)
                          .To(vm => vm.Duration)
                          .WithConversion(timeSpanConverter);

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

                bindingSet.Bind(TaskLabel)
                          .For(v => v.BindVisibility())
                          .To(vm => vm.HasProject)
                          .WithConversion(visibilityConverter);

                bindingSet.Apply();
            });
        }
    }
}
