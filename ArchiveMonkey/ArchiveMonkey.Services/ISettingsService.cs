using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Services
{
    public interface ISettingsService
    {
        ArchiveMonkeySettings GetSettings();

        void SaveSettings(ArchiveMonkeySettings settings);
    }
}
