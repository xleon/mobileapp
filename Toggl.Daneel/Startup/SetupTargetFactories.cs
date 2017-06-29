﻿using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.iOS.Platform;
using Toggl.Daneel.Binding;
using UIKit;

namespace Toggl.Daneel
{
    public partial class Setup : MvxIosSetup
    {
        protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            base.FillTargetFactories(registry);

            registry.RegisterCustomBindingFactory<UIBarButtonItem>(
                BarButtonAnimatedTintColorTargetBinding.BindingName,
                view => new BarButtonAnimatedTintColorTargetBinding(view)
            );

            registry.RegisterCustomBindingFactory<UIBarButtonItem>(
                BarButtonCommandTargetBinding.BindingName,
                view => new BarButtonCommandTargetBinding(view)
            );

            registry.RegisterCustomBindingFactory<UIBarButtonItem>(
                BarButtonEnabledTargetBinding.BindingName,
                view => new BarButtonEnabledTargetBinding(view)
            );

            registry.RegisterCustomBindingFactory<UIScrollView>(
                ScrollViewCurrentPageTargetBinding.BindingName,
                view => new ScrollViewCurrentPageTargetBinding(view)
            );

            registry.RegisterCustomBindingFactory<UIView>(
                ViewAnimatedBackgroundTargetBinding.BindingName,
                view => new ViewAnimatedBackgroundTargetBinding(view)
            );

            registry.RegisterCustomBindingFactory<UIView>(
                ViewAnimatedVisibilityTargetBinding.BindingName,
                view => new ViewAnimatedVisibilityTargetBinding(view)
            );
        }
    }
}
