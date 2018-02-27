using System.Windows;
using ArchiveMonkey.Settings.Views;

namespace ArchiveMonkey.Settings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(SettingsView content)
        {
            InitializeComponent();

            var context = new MainWindowContext();
            this.DataContext = context;
            context.ContentView = content;
        }
    }
}
