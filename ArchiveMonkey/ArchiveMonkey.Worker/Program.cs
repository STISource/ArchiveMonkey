using System;
using System.Collections.Generic;
using ArchiveMonkey.Services;
using Unity;

namespace ArchiveMonkey.Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            // check command line parameters
            if(args.Length != 1 && args.Length != 3)
            {
                Console.WriteLine("Invalid usage. Please use as follows:");
                Console.WriteLine("ArchiveMonkey.Worker.exe <server> [<username> optional] [<password> optional]");
                Console.WriteLine("Press enter to exit ...");
                Console.ReadLine();

                return;                
            }

            var login = new ArchivingLogin { Server = args[0] };
            if(args.Length == 3)
            {
                login.Username = args[1];
                login.Password = args[2];
            }

            // setup IoC Container
            var iocContainer = new UnityContainer();
            iocContainer.RegisterInstance(login);
            iocContainer.RegisterType<ISettingsService, JsonSettingsService>();            

            var settingsService = iocContainer.Resolve<ISettingsService>();
            var settings = settingsService.GetSettings();
            settings.ResolveDependencies();

            iocContainer.RegisterInstance(settings);
            iocContainer.RegisterInstance(new Queue<ArchivingAction>());
            iocContainer.RegisterType<IArchiveWatcher, DavidArchiveWatcher>();
            iocContainer.RegisterType<IArchivingHistoryService, NoSqlHistoryService>();
            iocContainer.RegisterInstance(iocContainer);
            iocContainer.RegisterInstance<IArchiver>(new DavidArchiver(iocContainer.Resolve<IArchivingHistoryService>()));
            iocContainer.RegisterType<Worker>();

            var worker = iocContainer.Resolve<Worker>();

            worker.Run();

            // just ensure the program keeps running
            while(true)
            {
                if(Console.ReadLine() == "exit")
                {
                    break;
                }
            }
        }
    }
}
