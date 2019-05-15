using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using Intents;
using IntentsUI;
using Toggl.Core;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.Models;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    using ShortcutSection = SectionModel<string, SiriShortcutViewModel>;

    public sealed partial class SiriShortcutsViewController : ReactiveViewController<SiriShortcutsViewModel>, IINUIAddVoiceShortcutViewControllerDelegate, IINUIEditVoiceShortcutViewControllerDelegate
    {
        private ISubject<Unit> refreshSubject = new Subject<Unit>();

        public SiriShortcutsViewController() : base(nameof(SiriShortcutsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            Title = Resources.Siri_Shortcuts;

            DescriptionLabel.Text = Resources.Siri_Shortcuts_Description;
            HeaderView.RemoveFromSuperview();
            HeaderView.BackgroundColor = Colors.Siri.HeaderBackground.ToNativeColor();
            TableView.TableHeaderView = HeaderView;
            HeaderView.TranslatesAutoresizingMaskIntoConstraints = false;
            HeaderView.WidthAnchor.ConstraintEqualTo(TableView.WidthAnchor).Active = true;

            TableView.BackgroundColor = Colors.Siri.HeaderBackground.ToNativeColor();
            TableView.TableFooterView = new UIView();

            var tableViewSource = new SiriShortcutsTableViewSource(TableView);
            TableView.Source = tableViewSource;

            refreshSubject.StartWith(Unit.Default)
                .SelectMany(getAllShortcuts())
                .SelectMany(toViewModels)
                .Select(toSections)
                .ObserveOn(new NSRunloopScheduler())
                .Subscribe(TableView.Rx().ReloadSections(tableViewSource))
                .DisposedBy(DisposeBag);

            tableViewSource.Rx().ModelSelected()
                .Subscribe(handleShortcutTap)
                .DisposedBy(DisposeBag);
        }

        private void handleShortcutTap(SiriShortcutViewModel shortcut)
        {
            if (!shortcut.IsActive)
            {
                if (shortcut.Type == SiriShortcutType.CustomStart)
                {
                    // Navigate to custom shortcut creation
                    return;
                }

                if (shortcut.Type == SiriShortcutType.CustomReport)
                {
                    ViewModel.NavigateToCustomReportShortcut.Execute();
                    return;
                }

                var intent = IosDependencyContainer.Instance.IntentDonationService.CreateIntent(shortcut.Type);
                var vc = new INUIAddVoiceShortcutViewController(new INShortcut(intent));
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
            else
            {
                var vc = new INUIEditVoiceShortcutViewController(shortcut.VoiceShortcut);
                vc.ModalPresentationStyle = UIModalPresentationStyle.FormSheet;
                vc.Delegate = this;
                PresentViewController(vc, true, null);
            }
        }

        private IObservable<IEnumerable<SiriShortcut>> getAllShortcuts()
        {
            return IosDependencyContainer.Instance.IntentDonationService.GetCurrentShortcuts()
                .Select(shortcuts => SiriShortcut.DefaultShortcuts
                    .Concat(shortcuts));
        }

        private IObservable<IEnumerable<SiriShortcutViewModel>> toViewModels(IEnumerable<SiriShortcut> shortcuts)
        {
            return shortcuts
                .Select(shortcut =>
                {
                    var projectId = stringToLong((string) shortcut.Parameters?[SiriShortcutParametersKey.ProjectId]);
                    if (shortcut.VoiceShortcut != null && projectId.HasValue)
                    {
                        return ViewModel.GetProject(projectId.Value)
                            .Select(project => new SiriShortcutViewModel(shortcut, project));
                    }

                    return Observable.Return(new SiriShortcutViewModel(shortcut));
                })
                .ToObservable().Merge().ToList();
        }

        private IEnumerable<ShortcutSection> toSections(IEnumerable<SiriShortcutViewModel> shortcuts)
        {
            var allShortcuts = shortcuts
                .Aggregate(new List<SiriShortcutViewModel>(), (acc, shortcut) =>
                {
                    if (shortcut.Type != SiriShortcutType.CustomStart && shortcut.Type != SiriShortcutType.CustomReport)
                    {
                        var index = acc.IndexOf(s => shortcut.Type == s.Type);
                        if (index != -1)
                        {
                            acc[index] = shortcut;
                            return acc;
                        }
                    }

                    acc.Add(shortcut);
                    return acc;
                })
                .OrderBy(shortcut => !shortcut.IsActive);

            return new[]
            {
                new ShortcutSection(
                    "Timer shortcuts",
                    allShortcuts.Where(s => s.IsTimerShortcut())
                ),
                new ShortcutSection(
                    "Reports shortcuts",
                    allShortcuts.Where(s => s.IsReportsShortcut())
                )
            };
        }

        private long? stringToLong(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return (long)Convert.ToDouble(str);
        }

        // IINUIAddVoiceShortcutViewControllerDelegate

        public void DidFinish(INUIAddVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIAddVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }

        // IINUIEditVoiceShortcutViewControllerDelegate

        public void DidUpdate(INUIEditVoiceShortcutViewController controller, INVoiceShortcut voiceShortcut, NSError error)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidDelete(INUIEditVoiceShortcutViewController controller, NSUuid deletedVoiceShortcutIdentifier)
        {
            refreshSubject.OnNext(Unit.Default);
            controller.DismissViewController(true, null);
        }

        public void DidCancel(INUIEditVoiceShortcutViewController controller)
        {
            controller.DismissViewController(true, null);
        }
    }
}

