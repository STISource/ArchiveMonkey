using System;
using System.Collections.Generic;
using ArchiveMonkey.Settings.Models;
using NLog;
using Unity;

namespace ArchiveMonkey.Worker
{
    public class Worker
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly ArchiveMonkeySettings settings;
        private readonly Queue<ArchivingAction> queue;
        private readonly UnityContainer iocContainer;
        private readonly ICollection<IArchiveWatcher> archiveWatchers;
        private readonly IArchiver archiver;

        private bool processing = false;

        public Worker(ArchiveMonkeySettings settings, Queue<ArchivingAction> queue, IArchiver archiver, UnityContainer iocContainer)
        {
            this.settings = settings;
            this.queue = queue;
            this.iocContainer = iocContainer;
            this.archiveWatchers = new List<IArchiveWatcher>();
            this.archiver = archiver;

            foreach (var action in this.settings.ArchivingActions)
            {
                // create one queue entry for every archiving action to ensure changes are processed that might have been missed since last program run
                this.queue.Enqueue(action);                
            }
        }        

        public void Run()
        {
            foreach(var action in this.settings.ArchivingActions)
            {
                var watcher = this.iocContainer.Resolve<IArchiveWatcher>();
                watcher.InputArchiveChanged += WatcherInputArchiveChanged;
                this.archiveWatchers.Add(this.iocContainer.Resolve<IArchiveWatcher>());
                watcher.Watch(action);
            }

            this.ProcessQueue();
        }

        private void ProcessQueue()
        {
            // ensure processing occurs only once at a time
            if(this.processing)
            {
                return;
            }

            this.processing = true;

            while (this.queue.Count > 0)
            {
                var action = this.queue.Dequeue();

                // if the next action is identical, just skip this one. Same archives doesn't need to be processed several times right after each other.
                if(action == this.queue.Peek())
                {
                    continue;
                }

                this.archiver.Archive(action);
            }

            this.processing = false;
        }

        private void WatcherInputArchiveChanged(object sender, EventArgs e)
        {
            var action = ((IArchiveWatcher)sender).WatchedArchivingAction;
            this.queue.Enqueue(action);

            this.ProcessQueue();
        }
    }
}
