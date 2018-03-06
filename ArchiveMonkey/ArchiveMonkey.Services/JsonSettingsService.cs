using System;
using System.IO;
using System.Runtime.Serialization.Json;
using ArchiveMonkey.Settings.Models;
using NLog;

namespace ArchiveMonkey.Services
{
    public class JsonSettingsService : ISettingsService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public readonly string SettingsPath = @"ArchiveMonkeySettings.json";

        public ArchiveMonkeySettings GetSettings()
        {
            if(File.Exists(this.SettingsPath))
            {
                ArchiveMonkeySettings settings = null;
                var serializer = new DataContractJsonSerializer(typeof(ArchiveMonkeySettings));

                using (var fileStream = File.OpenRead(this.SettingsPath))
                {
                    try
                    {
                        settings = (ArchiveMonkeySettings)serializer.ReadObject(fileStream);
                    }
                    catch(Exception ex)
                    {
                        settings = null;
                        logger.Error(ex, "Could not deserialize settings.");
                    }
                }

                return settings ?? new ArchiveMonkeySettings();
            }

            return new ArchiveMonkeySettings();
        }

        public void SaveSettings(ArchiveMonkeySettings settings)
        {
            var serializer = new DataContractJsonSerializer(typeof(ArchiveMonkeySettings));

            using(var fileStream = File.OpenWrite(this.SettingsPath))
            {
                serializer.WriteObject(fileStream, settings);
            }
        }
    }
}
