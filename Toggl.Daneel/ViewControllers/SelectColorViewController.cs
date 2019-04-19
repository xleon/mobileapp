using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using CoreGraphics;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Extensions.Reactive;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.Views;
using Toggl.Daneel.ViewSources;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [ModalDialogPresentation]
    public sealed partial class SelectColorViewController : ReactiveViewController<SelectColorViewModel>
    {
        private const int customColorEnabledHeightPad = 490;
        private const int customColorEnabledHeight = 365;
        private const int customColorDisabledHeight = 233;



        private ColorSelectionCollectionViewSource source;

        public SelectColorViewController()
            : base(nameof(SelectColorViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TitleLabel.Text = Resources.ProjectColor;
            SaveButton.SetTitle(Resources.Save, UIControlState.Normal);

            prepareViews();

            //Collection View
            ColorCollectionView.RegisterNibForCell(ColorSelectionViewCell.Nib, ColorSelectionViewCell.Identifier);
            source = new ColorSelectionCollectionViewSource(ViewModel.SelectableColors);
            ColorCollectionView.Source = source;
            ViewModel.SelectableColors
                .Subscribe(replaceColors)
                .DisposedBy(DisposeBag);

            source.ColorSelected
                .Subscribe(ViewModel.SelectColor.Inputs)
                .DisposedBy(DisposeBag);

            // Commands
            SaveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            CloseButton.Rx()
                .BindAction(ViewModel.Close)
                .DisposedBy(DisposeBag);

            // Picker view
            PickerView.Rx().Hue()
                .Subscribe(ViewModel.SetHue.Inputs)
                .DisposedBy(DisposeBag);


            PickerView.Rx().Saturation()
                .Subscribe(ViewModel.SetSaturation.Inputs)
                .DisposedBy(DisposeBag);

            SliderView.Rx().Value()
                .Select(v => 1 - v)
                .Subscribe(ViewModel.SetValue.Inputs)
                .DisposedBy(DisposeBag);

            ViewModel.Hue
                .Subscribe(PickerView.Rx().HueObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Saturation
                .Subscribe(PickerView.Rx().SaturationObserver())
                .DisposedBy(DisposeBag);

            ViewModel.Value
                .Subscribe(PickerView.Rx().ValueObserver())
                .DisposedBy(DisposeBag);
        }

        private void prepareViews()
        {
            var screenWidth = UIScreen.MainScreen.Bounds.Width;
            PreferredContentSize = new CGSize
            {
                // ScreenWidth - 32 for 16pt margins on both sides
                Width = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
                    ? 0
                    : screenWidth > 320 ? screenWidth - 32 : 312,
                Height = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad
                    ? (ViewModel.AllowCustomColors ? customColorEnabledHeightPad : customColorDisabledHeight)
                    : (ViewModel.AllowCustomColors ? customColorEnabledHeight : customColorDisabledHeight)
            };

            if (!ViewModel.AllowCustomColors)
            {
                SliderView.RemoveFromSuperview();
                PickerView.RemoveFromSuperview();
                SliderBackgroundView.RemoveFromSuperview();
                return;
            }

            // Remove track
            SliderView.SetMinTrackImage(new UIImage(), UIControlState.Normal);
            SliderView.SetMaxTrackImage(new UIImage(), UIControlState.Normal);
        }

        private void replaceColors(IEnumerable<SelectableColorViewModel> colors)
        {
            source.SetNewColors(colors);
            ColorCollectionView.ReloadData();
        }
    }
}

