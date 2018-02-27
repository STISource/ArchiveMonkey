using ArchiveMonkey.Settings.Models;
using ArchiveMonkey.Settings.Services;
using System.ComponentModel;

namespace ArchiveMonkey.Settings.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        private readonly ISettingsService settingsService;

        private ArchiveMonkeySettings settings;

        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            this.Settings = settingsService.GetSettings();
        }

        public ArchiveMonkeySettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                if(this.settings != value)
                {
                    this.settings = value;
                    this.NotifyChanged("Settings");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
