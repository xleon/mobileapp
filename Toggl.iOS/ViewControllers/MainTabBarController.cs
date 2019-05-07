﻿using System;
using System.Collections.Generic;
using System.Linq;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.iOS.Presentation;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    [MvxRootPresentation(WrapInNavigationController = false)]
    public class MainTabBarController : UITabBarController
    {
        public MainTabBarViewModel ViewModel { get; set; }

        private static readonly Dictionary<Type, string> imageNameForType = new Dictionary<Type, string>
        {
            { typeof(MainViewModel), "icTime" },
            { typeof(ReportsViewModel), "icReports" },
            { typeof(CalendarViewModel), "icCalendar" },
            { typeof(SettingsViewModel), "icSettings" }
        };

        public MainTabBarController(MainTabBarViewModel viewModel)
        {
            ViewModel = viewModel;
            ViewControllers = ViewModel.Tabs.Select(createTabFor).ToArray();

            UIViewController createTabFor(ViewModel childViewModel)
            {
                var childViewController = createViewControllerFor(childViewModel);
                var item = new UITabBarItem();
                item.Title = "";
                item.Image = UIImage.FromBundle(imageNameForType[childViewModel.GetType()]);
                childViewController.TabBarItem = item;
                return new UINavigationController(childViewController);
            }
        }

        private UIViewController createViewControllerFor(ViewModel viewModel)
        {
            switch (viewModel)
            {
                case MainViewModel mainViewModel:
                    var mainViewController = new MainViewController();
                    mainViewController.ViewModel = mainViewModel;
                    return mainViewController;
                case ReportsViewModel reportsViewModel:
                    var reportsViewController = new ReportsViewController();
                    reportsViewController.ViewModel = reportsViewModel;
                    return reportsViewController;
                case CalendarViewModel calendarViewModel:
                    var calendarViewController = new CalendarViewController();
                    calendarViewController.ViewModel = calendarViewModel;
                    return calendarViewController;
                case SettingsViewModel settingsViewModel:
                    var settingsViewController = new SettingsViewController();
                    settingsViewController.ViewModel = settingsViewModel;
                    return settingsViewController;
                default:
                    throw new Exception($"Cannot create view controller for view model of type {viewModel.GetType().Name}");
            }
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TabBar.Translucent = UIDevice.CurrentDevice.CheckSystemVersion(11, 0);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            recalculateTabBarInsets();
        }

        public override void TraitCollectionDidChange(UITraitCollection previousTraitCollection)
        {
            base.TraitCollectionDidChange(previousTraitCollection);
            recalculateTabBarInsets();
        }

        public override void ItemSelected(UITabBar tabbar, UITabBarItem item)
        {
            var targetViewController = ViewControllers.Single(vc => vc.TabBarItem == item);

            if (targetViewController is UINavigationController navigationController
                && navigationController.TopViewController is ReportsViewController)
            {
                ViewModel.StartReportsStopwatch();
            }

            if (targetViewController == SelectedViewController
                && tryGetScrollableController() is IScrollableToTop scrollable)
            {
                scrollable.ScrollToTop();
            }

            UIViewController tryGetScrollableController()
            {
                if (targetViewController is IScrollableToTop)
                    return targetViewController;

                if (targetViewController is UINavigationController nav)
                    return nav.TopViewController;

                return null;
            }
        }

        private void recalculateTabBarInsets()
        {
            ViewControllers.ToList()
                           .ForEach(vc =>
            {
                if (TraitCollection.HorizontalSizeClass == UIUserInterfaceSizeClass.Compact)
                {
                    vc.TabBarItem.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
                }
                else
                {
                    vc.TabBarItem.ImageInsets = new UIEdgeInsets(0, 0, 0, 0);
                }
            });
        }
    }
}