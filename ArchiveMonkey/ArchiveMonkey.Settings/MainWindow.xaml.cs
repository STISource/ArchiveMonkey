using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace ArchiveMonkey.Settings
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow(UserControl content)
        {
            InitializeComponent();

            var context = new MainWindowContext();
            this.DataContext = context;
            context.ContentView = content;
        }
    }
}
