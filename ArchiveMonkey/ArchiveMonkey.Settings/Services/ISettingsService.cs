using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Settings.Services
{
    public interface ISettingsService
    {
        ArchiveMonkeySettings GetSettings();

        void SaveSettings(ArchiveMonkeySettings settings);
    }
}
