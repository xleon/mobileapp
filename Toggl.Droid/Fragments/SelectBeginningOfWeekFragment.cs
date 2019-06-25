using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public sealed partial class SelectBeginningOfWeekFragment : ReactiveDialogFragment<SelectBeginningOfWeekViewModel>
    {
        public SelectBeginningOfWeekFragment() { }

        public SelectBeginningOfWeekFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) { }

        private SimpleAdapter<SelectableBeginningOfWeekViewModel> adapter;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.SelectBeginningOfWeekFragment, null);
            InitializeViews(view);

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            setupRecyclerView();

            adapter.ItemTapObservable
                .Subscribe(ViewModel.SelectBeginningOfWeek.Inputs)
                .DisposedBy(DisposeBag);
        }

        public override void OnResume()
        {
            base.OnResume();

            Dialog.Window.SetDefaultDialogLayout(Activity, Context, heightDp: 400);
        }

        private void setupRecyclerView()
        {
            recyclerView.SetLayoutManager(new LinearLayoutManager(Context));

            adapter = new SimpleAdapter<SelectableBeginningOfWeekViewModel>(
                Resource.Layout.SelectBeginningOfWeekFragmentCell,
                BeginningOfWeekViewHolder.Create);

            adapter.Items = ViewModel.BeginningOfWeekCollection;

            recyclerView.SetAdapter(adapter);
        }
    }
}
