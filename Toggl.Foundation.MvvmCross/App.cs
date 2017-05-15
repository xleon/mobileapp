using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Foundation.MvvmCross
{
    public class App : MvxApplication
    {
        public override void Initialize()
            => RegisterAppStart<LoginViewModel>();
    }
}
