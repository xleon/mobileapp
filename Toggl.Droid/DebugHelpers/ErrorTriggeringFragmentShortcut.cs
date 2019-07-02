#if !USE_PRODUCTION_API
using System;
using Toggl.Core.UI.Extensions;
using Toggl.Droid.Debug;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Fragments
{
    public partial class SettingsFragment
    {
        public override void OnResume()
        {
            base.OnResume();

            aboutView.Rx().LongPress()
                .Subscribe(showErrorTriggeringView)
                .DisposedBy(DisposeBag);
        }

        private void showErrorTriggeringView()
        {
            new ErrorTriggeringFragment()
                .Show(FragmentManager, nameof(ErrorTriggeringFragment));
        }
    }
}
#endif
