using ArchiveMonkey.Settings.Models;
using System;

namespace ArchiveMonkey.Worker
{
    public interface IArchiveWatcher
    {
        ArchivingActionTemplate WatchedArchivingActionTemplate { get; }

        void Watch(ArchivingActionTemplate archive);

        void Stop();

        event ArchiveChangedEventHandler InputArchiveChanged;
    }

    public class ArchiveChangedEventArgs : EventArgs
    {
        public ArchivingAction ActionToPerform { get; private set; }

        public ArchiveChangedEventArgs(ArchivingAction actionToPerform)
        {
            this.ActionToPerform = actionToPerform;
        }
    }

    public delegate void ArchiveChangedEventHandler(object sender, ArchiveChangedEventArgs args);
}
