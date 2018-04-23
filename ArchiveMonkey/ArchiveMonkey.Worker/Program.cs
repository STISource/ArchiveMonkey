using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ArchiveMonkey.Services;
using NLog;
using Unity;

namespace ArchiveMonkey.Worker
{
    class Program
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public const string StopFile = @"stop";

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

            if(File.Exists(StopFile))
            {
                File.Delete(StopFile);
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

            var queue = new Queue<ArchivingAction>();
            iocContainer.RegisterInstance(queue);
            iocContainer.RegisterInstance(settings);
            iocContainer.RegisterType<IArchiveWatcher, DavidArchiveWatcher>();
            iocContainer.RegisterType<IArchivingHistoryService, NoSqlHistoryService>();
            iocContainer.RegisterType<IFilterService, DavidFilterService>();
            iocContainer.RegisterInstance(iocContainer);
            iocContainer.RegisterType<IArchiver, DavidArchiver>();
            iocContainer.RegisterType<Worker>();

            var worker = iocContainer.Resolve<Worker>();

            worker.Run();

            // just ensure the program keeps running
            while(true)
            {
                if(File.Exists(StopFile))
                {
                    logger.Info("Found stop file {0} ... Stop watching of folders.", StopFile);
                    worker.StopWatching();

                    logger.Info("Processing of queue still running? {0}", worker.Processing);
                    if(!worker.Processing)
                    {
                        logger.Info("Processing queue finished. Stopping program...");
                        break;
                    }                    
                }

                Thread.Sleep(500);
            }
        }
    }
}
