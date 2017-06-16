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

            registry.RegisterCustomBindingFactory<UIScrollView>(
                ScrollViewCurrentPageTargetBinding.BindingName,
                view => new ScrollViewCurrentPageTargetBinding(view)
            );
        }
    }
}