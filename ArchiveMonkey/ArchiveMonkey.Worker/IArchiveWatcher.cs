using ArchiveMonkey.Settings.Models;
using System;
using System.Collections.Generic;

namespace ArchiveMonkey.Worker
{
    public interface IArchiveWatcher
    {
        IEnumerable<ArchivingActionTemplate> WatchedArchivingActionTemplates { get; }

        void Watch(IEnumerable<ArchivingActionTemplate> archive);

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
