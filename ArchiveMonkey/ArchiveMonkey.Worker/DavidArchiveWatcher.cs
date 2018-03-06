using System;
using System.Collections.Generic;
using System.IO;
using ArchiveMonkey.Settings.Models;
using NLog;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiveWatcher : IArchiveWatcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private const string mailExtension = ".001";
        private FileSystemWatcher watcher;
        private IList<string> filesAwaitingChangedNotification = new List<string>();        

        public ArchivingActionTemplate WatchedArchivingActionTemplate { get; private set; }

        public event ArchiveChangedEventHandler InputArchiveChanged;

        public void Watch(ArchivingActionTemplate actionTempalte)
        {
            this.WatchedArchivingActionTemplate = actionTempalte;

            this.watcher = new FileSystemWatcher();
            if(!Directory.Exists(actionTempalte.InputArchive.Path))
            {
                throw new ArgumentException("Invalid input path");
            }
            this.watcher.Path = actionTempalte.InputArchive.Path;
            this.watcher.IncludeSubdirectories = false;

            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName;
            this.watcher.Filter = "*" + mailExtension; // should be sufficient to identify new incoming email

            // Add event handlers.
            this.watcher.Changed += WatchedFolderChanged;
            this.watcher.Created += WatchedFolderChanged;                  

            this.watcher.EnableRaisingEvents = true;

            logger.Info("Watching {0} started.", actionTempalte.InputArchive.Path);            
        }

        public void Stop()
        {
            this.watcher.EnableRaisingEvents = false;
            this.watcher.Changed -= WatchedFolderChanged;
            this.watcher.Created -= WatchedFolderChanged;
            this.watcher.Deleted -= WatchedFolderChanged;            
            this.watcher.Dispose();

            logger.Info("Watching {0} stopped.", this.WatchedArchivingActionTemplate.InputArchive.Path);
        }        

        private void WatchedFolderChanged(object sender, FileSystemEventArgs e)
        {
            logger.Debug("File system action for {0}: {1}.", e.FullPath, e.ChangeType);
            if (!e.Name.ToLower().StartsWith("archive"))
            {
                // ensure change notification is only triggered when a file has been created (so changes are basically done)
                switch (e.ChangeType) 
                {
                    case WatcherChangeTypes.Created:
                        this.filesAwaitingChangedNotification.Add(e.FullPath);
                        this.RaiseInputArchiveChanged(
                            ArchivingAction.FromTemplate(
                                this.WatchedArchivingActionTemplate,
                                e.FullPath));
                        logger.Debug("ArchiveChanged raised for {0}.", e.FullPath);
                        break;

                    case WatcherChangeTypes.Changed:
                        if(this.filesAwaitingChangedNotification.Contains(e.FullPath))
                        {
                            this.filesAwaitingChangedNotification.Remove(e.FullPath);                                                        
                        }
                        
                        break;
                }                
            }
        }        

        private void RaiseInputArchiveChanged(ArchivingAction action)
        {
            this.InputArchiveChanged?.Invoke(this, new ArchiveChangedEventArgs(action));
        }
    }
}
