﻿using Toggl.Daneel.Binding;
using UIKit;

namespace Toggl.Daneel.Extensions
{  
    public static class ViewBindingExtensions
    {

        public static string BindAnimatedTintColor(this UIBarButtonItem self)
            => BarButtonAnimatedTintColorTargetBinding.BindingName;

        public static string BindCommand(this UIBarButtonItem self)
            => BarButtonCommandTargetBinding.BindingName;

        public static string BindEnabled(this UIBarButtonItem self)
            => BarButtonEnabledTargetBinding.BindingName;

        public static string BindCurrentPage(this UIScrollView self)
            => ScrollViewCurrentPageTargetBinding.BindingName;

        public static string BindAnimatedBackground(this UIView self)
            => ViewAnimatedBackgroundTargetBinding.BindingName;

        public static string BindAnimatedVisibility(this UIView self)
            => ViewAnimatedVisibilityTargetBinding.BindingName;
    }
}
