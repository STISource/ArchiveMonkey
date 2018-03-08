using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
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

                using (var fileStream = new FileStream(this.SettingsPath, FileMode.Open))
                {
                    try
                    {
                        using (var jsonReader = JsonReaderWriterFactory.CreateJsonReader(fileStream, Encoding.GetEncoding("utf-8"), XmlDictionaryReaderQuotas.Max, null))
                        {
                            settings = (ArchiveMonkeySettings)serializer.ReadObject(jsonReader);
                        }
                    }
                    catch (Exception ex)
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

            using (var fileStream = new FileStream(this.SettingsPath, FileMode.Create))
            {
                using (var jsonWriter = JsonReaderWriterFactory.CreateJsonWriter(fileStream, Encoding.GetEncoding("utf-8")))
                {

                    serializer.WriteObject(jsonWriter, settings);
                    jsonWriter.Flush();
                }
            }
        }
    }
}
