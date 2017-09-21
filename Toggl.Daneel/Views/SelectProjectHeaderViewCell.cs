using System;
using Foundation;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS.Views;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;

namespace Toggl.Daneel.Views
{
    public sealed partial class SelectProjectHeaderViewCell : MvxTableViewHeaderFooterView
    {
        public static readonly NSString Key = new NSString(nameof(SelectProjectHeaderViewCell));
        public static readonly UINib Nib;

        public bool TopSeparatorHidden
        {
            get => TopSeparator.Hidden;
            set => TopSeparator.Hidden = value;
        }

        static SelectProjectHeaderViewCell()
        {
            Nib = UINib.FromName(nameof(SelectProjectHeaderViewCell), NSBundle.MainBundle);
        }

        protected SelectProjectHeaderViewCell(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.DelayBind(() =>
            {
                var bindingSet = this.CreateBindingSet<SelectProjectHeaderViewCell, ProjectSuggestionCollection>();

                bindingSet.Bind(WorkspaceLabel).For(v => v.Text).To(vm => vm.Workspace);
                
                bindingSet.Apply();
            });
        }
    }
}
