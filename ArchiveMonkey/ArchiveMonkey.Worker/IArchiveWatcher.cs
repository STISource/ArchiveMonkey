using ArchiveMonkey.Settings.Models;
using System;

namespace ArchiveMonkey.Worker
{
    public interface IArchiveWatcher
    {
        ArchivingAction WatchedArchivingAction { get; }

        void Watch(ArchivingAction archive);

        void Stop();

        event EventHandler InputArchiveChanged;
    }
}
