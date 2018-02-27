using System.Windows;
using ArchiveMonkey.Settings.Services;
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
        public App()
        {
            var container = new UnityContainer();
            container.RegisterType<ISettingsService, JsonSettingsService>();
            container.RegisterType<SettingsViewModel>();
            container.RegisterType<SettingsView>();

            new MainWindow(container.Resolve<SettingsView>()).Show();
        }        
    }
}
