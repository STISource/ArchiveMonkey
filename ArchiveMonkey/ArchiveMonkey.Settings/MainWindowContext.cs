using System.ComponentModel;
using System.Windows.Controls;

namespace ArchiveMonkey.Settings
{
    /// <summary>
    /// This viewmodel is just a host viewmodel needed for the mainwindow.
    /// It is the basis for dynamically loading content.
    /// </summary>
    public class MainWindowContext : INotifyPropertyChanged
    {
        private UserControl contentView;
        
        public UserControl ContentView
        {
            get
            {
                return this.contentView;
            }

            set
            {
                if(this.contentView != value)
                {
                    this.contentView = value;

                }

            }

        }

        public event PropertyChangedEventHandler PropertyChanged;        

        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
