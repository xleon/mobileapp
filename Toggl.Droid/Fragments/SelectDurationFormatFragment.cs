using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Extensions;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SelectDurationFormatFragment : ReactiveDialogFragment<SelectDurationFormatViewModel>
    {
        public SelectDurationFormatFragment() { }

        public SelectDurationFormatFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SelectDurationFormatFragment, null);

            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            
            selectDurationRecyclerAdapter.Items = ViewModel.DurationFormats;
            
            selectDurationRecyclerAdapter.ItemTapObservable
                .Subscribe(ViewModel.SelectDurationFormat.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: 268);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing) return;
            DisposeBag.Dispose();
        }
    }
}
