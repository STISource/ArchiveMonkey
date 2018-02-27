using System.Windows.Controls;
using ArchiveMonkey.Settings.ViewModels;

namespace ArchiveMonkey.Settings.Views
{
    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl
    {
        public SettingsView(SettingsViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
