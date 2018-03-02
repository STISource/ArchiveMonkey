using System.Collections.Generic;
using System.Threading;
using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;
using Unity;

namespace ArchiveMonkey.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            // setup IoC Container
            var iocContainer = new UnityContainer();
            iocContainer.RegisterType<ISettingsService, JsonSettingsService>();

            var settingsService = iocContainer.Resolve<ISettingsService>();
            var settings = settingsService.GetSettings();
            settings.ResolveDependencies();

            iocContainer.RegisterInstance<ArchiveMonkeySettings>(settings);
            iocContainer.RegisterInstance(new Queue<ArchivingAction>());
            iocContainer.RegisterType<IArchiveListener, DavidArchiveListener>();
            iocContainer.RegisterInstance(iocContainer);
            iocContainer.RegisterType<Worker>();           


            // just ensure the program keeps running
            while(true)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
