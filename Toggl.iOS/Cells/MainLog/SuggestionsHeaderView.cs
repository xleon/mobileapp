﻿using System;

using Foundation;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.iOS.Shared.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.Cells.MainLog
{
    public partial class SuggestionsHeaderView : BaseTableHeaderFooterView<SuggestionsHeaderViewModel>
    {
        public static readonly string Identifier = "SuggestionsHeaderView";
        public static readonly UINib Nib;

        private const float titleSize = 12;

        static SuggestionsHeaderView()
        {
            Nib = UINib.FromName("SuggestionsHeaderView", NSBundle.MainBundle);
        }

        protected SuggestionsHeaderView(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();
            TitleLabel.Text = Resources.SuggestionsHeader;
        }
    }
}
