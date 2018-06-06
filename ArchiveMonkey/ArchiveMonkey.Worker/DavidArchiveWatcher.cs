using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;
using NLog;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiveWatcher : IArchiveWatcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string mailExtension = ".001";

        private readonly IFilterService filterService;

        private FileSystemWatcher watcher;
        private IList<string> filesAwaitingChangedNotification = new List<string>();        

        public IEnumerable<ArchivingActionTemplate> WatchedArchivingActionTemplates { get; private set; }

        public event ArchiveChangedEventHandler InputArchiveChanged;

        public DavidArchiveWatcher(IFilterService filterService)
        {
            this.filterService = filterService;
        }

        public void Watch(IEnumerable<ArchivingActionTemplate> actionTempaltes)
        {
            this.WatchedArchivingActionTemplates = actionTempaltes.OrderBy(x => x.Sequence).ToList();
            var sourceArchive = this.WatchedArchivingActionTemplates.First().InputArchive;

            this.watcher = new FileSystemWatcher();
            if(!Directory.Exists(sourceArchive.FullLocalPath))
            {
                logger.Debug("Path does not exist: {0}", sourceArchive.Path);
                throw new ArgumentException("Invalid input path");
            }
            this.watcher.Path = sourceArchive.FullLocalPath;            
            this.watcher.IncludeSubdirectories = false;
            
            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.watcher.Filter = "*" + mailExtension; // should be sufficient to identify new incoming email
                        
            this.watcher.Changed += WatchedFolderChanged;
            this.watcher.Created += WatchedFolderChanged;    
            this.watcher.Renamed += WatchedFolderChanged;
            this.watcher.Error += WatcherError;

            this.watcher.EnableRaisingEvents = true;

            logger.Info("Watching {0} for {1} started.", this.watcher.Path, sourceArchive.DisplayName);            
        }        

        public void Stop()
        {
            this.watcher.EnableRaisingEvents = false;
            this.watcher.Changed -= WatchedFolderChanged;
            this.watcher.Created -= WatchedFolderChanged;
            this.watcher.Renamed -= WatchedFolderChanged;
            this.watcher.Error -= WatcherError;
            this.watcher.Dispose();

            logger.Info("Watching {0} stopped.", this.WatchedArchivingActionTemplates.First().InputArchive.Path);
        }        

        private void WatchedFolderChanged(object sender, FileSystemEventArgs e)
        {
            logger.Debug("File system action for {0}: {1}.", e.FullPath, e.ChangeType);
            if (!e.Name.ToLower().StartsWith("archive"))
            {
                // we need a change information if a new item is created. however david normally causes to file system notifications. one for the create. and a change when it's done.
                // so only notify if the action is finished.
                switch (e.ChangeType) 
                {
                    case WatcherChangeTypes.Created:
                        this.filesAwaitingChangedNotification.Add(e.FullPath);                                                
                        break;

                    case WatcherChangeTypes.Changed:
                        if(this.filesAwaitingChangedNotification.Contains(e.FullPath))
                        {                            
                            this.filesAwaitingChangedNotification.Remove(e.FullPath);
                            logger.Debug("Raise Archive changed for {0}.", e.FullPath);

                            foreach (var actionTemplate in this.WatchedArchivingActionTemplates)
                            {
                                this.RaiseInputArchiveChanged(
                                    ArchivingAction.FromTemplate(
                                        actionTemplate,
                                        this.filterService,
                                        e.FullPath));
                            }
                        }
                        
                        break;
                }                
            }
        }

        private void WatcherError(object sender, ErrorEventArgs e)
        {
            logger.Error(e.GetException());
        }

        private void RaiseInputArchiveChanged(ArchivingAction action)
        {
            this.InputArchiveChanged?.Invoke(this, new ArchiveChangedEventArgs(action));
        }
    }
}
