using System;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    [MvxDialogFragmentPresentation(AddToBackStack = true)]
    public sealed partial class TermsOfServiceFragment : ReactiveDialogFragment<TermsOfServiceViewModel>
    {
        public TermsOfServiceFragment() { }

        public TermsOfServiceFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.TermsOfServiceFragment, null);

            InitializeViews(view);
            bindViews();

            return view;
        }

        private void bindViews()
        {
            privacyPolicyTextView.Rx()
                .BindAction(ViewModel.ViewPrivacyPolicy)
                .DisposedBy(DisposeBag);

            termsOfServiceTextView.Rx()
                .BindAction(ViewModel.ViewTermsOfService)
                .DisposedBy(DisposeBag);

            acceptButton.Rx()
                .BindAction(ViewModel.Close, _ => true)
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: 350);
        }

        public override void OnCancel(IDialogInterface dialog)
        {
            ViewModel.Close.Execute(false);
        }
    }
}
