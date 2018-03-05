using System;
using System.IO;
using ArchiveMonkey.Settings.Models;
using NLog;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiveWatcher : IArchiveWatcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private FileSystemWatcher watcher;

        public ArchivingAction WatchedArchivingAction { get; private set; }

        public event EventHandler InputArchiveChanged;
        
        public void Watch(ArchivingAction archive)
        {
            this.WatchedArchivingAction = archive;

            this.watcher = new FileSystemWatcher();
            this.watcher.Path = archive.InputArchive.Path;

            this.watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            // Add event handlers.
            this.watcher.Changed += WatchedFolderChanged;
            this.watcher.Created += WatchedFolderChanged;
            this.watcher.Deleted += WatchedFolderChanged;
            this.watcher.Renamed += FolderContentItemRenamed;

            this.watcher.EnableRaisingEvents = true;

            logger.Info("Watching {0} started", archive.InputArchive.Path);            
        }

        public void Stop()
        {
            this.watcher.EnableRaisingEvents = false;
            this.watcher.Changed -= WatchedFolderChanged;
            this.watcher.Created -= WatchedFolderChanged;
            this.watcher.Deleted -= WatchedFolderChanged;
            this.watcher.Renamed -= FolderContentItemRenamed;
            this.watcher.Dispose();

            logger.Info("Watching {0} stopped", this.WatchedArchivingAction.InputArchive.Path);
        }

        private void FolderContentItemRenamed(object sender, RenamedEventArgs e)
        {
            logger.Debug("File system action for {0}: {1}", e.Name, e.ChangeType);
        }

        private void WatchedFolderChanged(object sender, FileSystemEventArgs e)
        {
            logger.Debug("File system action for {0}: {1}", e.Name, e.ChangeType);
        }        

        private void RaiseInputArchiveChanged()
        {
            this.InputArchiveChanged?.Invoke(this, new EventArgs());
        }
    }
}
