﻿using System.Windows;
using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.ViewModels;
using ArchiveMonkey.Settings.Views;
using Unity;

namespace ArchiveMonkey.Settings
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var container = new UnityContainer();
            container.RegisterType<ISettingsService, JsonSettingsService>();
            container.RegisterType<IFilterService, DavidFilterService>();
            container.RegisterType<SettingsViewModel>();
            container.RegisterType<SettingsView>();

            new MainWindow(container.Resolve<SettingsView>()).Show();
        }
    }
}
