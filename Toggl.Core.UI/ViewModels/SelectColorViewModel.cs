using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Parameters;
using Toggl.Core.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Colors = Toggl.Core.UI.Helper.Colors;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SelectColorViewModel : ViewModel<ColorParameters, Color>
    {
        private readonly INavigationService navigationService;
        private readonly IRxActionFactory rxActionFactory;

        private Color defaultColor;
        private IObservable<Color> customColor;
        private BehaviorSubject<Color> selectedColor = new BehaviorSubject<Color>(Colors.Transparent);

        private BehaviorSubject<float> hue { get; } = new BehaviorSubject<float>(0.0f);
        private BehaviorSubject<float> saturation { get; } = new BehaviorSubject<float>(0.0f);
        private BehaviorSubject<float> value { get; } = new BehaviorSubject<float>(0.375f);

        public bool AllowCustomColors { get; private set; }

        public IObservable<IEnumerable<SelectableColorViewModel>> SelectableColors { get; }
        public IObservable<float> Hue { get; }
        public IObservable<float> Saturation { get; }
        public IObservable<float> Value { get; }

        public UIAction Save { get; }
        public UIAction Close { get; }
        public InputAction<float> SetHue { get; }
        public InputAction<float> SetSaturation { get; }
        public InputAction<float> SetValue { get; }
        public InputAction<Color> SelectColor { get; }

        public SelectColorViewModel(INavigationService navigationService, IRxActionFactory rxActionFactory)
        {
            Ensure.Argument.IsNotNull(navigationService, nameof(navigationService));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));

            this.navigationService = navigationService;
            this.rxActionFactory = rxActionFactory;

            // Public properties
            Hue = hue.AsObservable();
            Saturation = saturation.AsObservable();
            Value = value.AsObservable();

            Save = rxActionFactory.FromAsync(save);
            Close = rxActionFactory.FromAsync(close);
            SetHue = rxActionFactory.FromAction<float>(hue.OnNext);
            SetSaturation = rxActionFactory.FromAction<float>(saturation.OnNext);
            SetValue = rxActionFactory.FromAction<float>(value.OnNext);
            SelectColor = rxActionFactory.FromAction<Color>(selectColor);

            customColor = Observable
                .CombineLatest(hue, saturation, value, Colors.FromHSV)
                .Do(selectedColor.OnNext);

            var availableColors = Observable.Return(Colors.DefaultProjectColors)
                .CombineLatest(customColor, combineAllColors);

            SelectableColors = availableColors
                .CombineLatest(selectedColor, updateSelectableColors);
        }

        public override Task Initialize(ColorParameters parameter)
        {
            defaultColor = parameter.Color;
            AllowCustomColors = parameter.AllowCustomColors;

            var noColorsSelected = Colors.DefaultProjectColors.None(color => color == defaultColor);

            if (noColorsSelected)
            {
                if (AllowCustomColors)
                {
                    var colorComponents = defaultColor.GetHSV();
                    hue.OnNext(colorComponents.hue);
                    saturation.OnNext(colorComponents.saturation);
                    value.OnNext(colorComponents.value);
                }
                else
                {
                    selectedColor.OnNext(Colors.DefaultProjectColors.First());
                }
            }
            else
            {
                selectedColor.OnNext(defaultColor);
            }

            return base.Initialize(parameter);
        }

        private IEnumerable<Color> combineAllColors(Color[] defaultColors, Color custom)
        {
            if (AllowCustomColors)
            {
                return defaultColors.Concat(new[] { custom });
            }

            return defaultColors;
        }

        private void selectColor(Color color)
        {
            selectedColor.OnNext(color);

            if (!AllowCustomColors)
                save();
        }

        private IEnumerable<SelectableColorViewModel> updateSelectableColors(IEnumerable<Color> availableColors, Color selectedColor)
            => availableColors.Select(color => new SelectableColorViewModel(color, color == selectedColor));

        private Task close()
            => Finish(defaultColor);

        private Task save()
            => Finish(selectedColor.Value);
    }
}
