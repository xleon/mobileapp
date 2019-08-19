using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Extensions;
using Toggl.Droid.Extensions;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
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

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            reviewTheTermsTextView.Text = Shared.Resources.ReviewTheTerms;
            termsMessageTextView.Text = Shared.Resources.TermsOfServiceDialogMessageDroid;
            termsOfServiceTextView.Text = Shared.Resources.TermsOfService;
            andTextView.Text = Shared.Resources.And;
            privacyPolicyTextView.Text = Shared.Resources.PrivacyPolicy;
            acceptButton.Text = Shared.Resources.IAgree;

            bindViews();
        }

        private void bindViews()
        {
            privacyPolicyTextView.Rx()
                .BindAction(ViewModel.ViewPrivacyPolicy)
                .DisposedBy(DisposeBag);

            termsOfServiceTextView.Rx()
                .BindAction(ViewModel.ViewTermsOfService)
                .DisposedBy(DisposeBag);

            acceptButton.Rx().Tap()
                .Subscribe(() => ViewModel.Close(true))
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: 350);
        }
    }
}
