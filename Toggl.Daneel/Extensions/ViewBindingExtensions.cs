﻿using Toggl.Daneel.Binding;
using UIKit;

namespace Toggl.Daneel.Extensions
{  
    public static class ViewBindingExtensions
    {

        public static string BindCurrentPage(this UIScrollView self)
            => ScrollViewCurrentPageTargetBinding.BindingName;
    }
}