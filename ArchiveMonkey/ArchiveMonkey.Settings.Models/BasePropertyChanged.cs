using System.ComponentModel;

namespace ArchiveMonkey.Settings.Models
{
    public class BasePropertyChanged : INotifyPropertyChanged
    {
        public void RaisePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
