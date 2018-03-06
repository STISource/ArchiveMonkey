using System;
using System.Collections.Generic;
using System.Threading;
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

            foreach (var action in this.settings.ArchivingActionTemplates)
            {
                // create one queue entry for every archiving action to ensure changes are processed that might have been missed since last program run
                this.queue.Enqueue(ArchivingAction.FromTemplate(action));
            }
        }        

        public void Run()
        {
            logger.Info("Worker starting ...");

            foreach(var action in this.settings.ArchivingActionTemplates)
            {
                IArchiveWatcher watcher = null;
                try
                {
                    watcher = this.iocContainer.Resolve<IArchiveWatcher>();
                    watcher.InputArchiveChanged += WatcherInputArchiveChanged;
                    watcher.Watch(action);
                    this.archiveWatchers.Add(watcher);
                }
                catch(ArgumentException)
                {                    
                    watcher.InputArchiveChanged -= WatcherInputArchiveChanged;                    
                }
            }            

            logger.Info("Worker started.");
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

            logger.Info("Processing worker queue started.");

            while (this.queue.Count > 0)
            {
                var action = this.queue.Dequeue();

                // wait a little, because sometimes the same action is enqued mulitple times
                Thread.Sleep(200);

                var nextAction = this.queue.Count > 0 ? this.queue.Peek() : (ArchivingAction)null;

                // if the next action is identical, just skip this one. Same archives doesn't need to be processed several times right after each other.
                if(this.ActionsAreIdentical(action, nextAction))
                {
                    continue;
                }

                this.archiver.Archive(action);
            }

            logger.Info("Processing worker queue finished.");

            this.processing = false;
        }

        private void WatcherInputArchiveChanged(object sender, ArchiveChangedEventArgs e)
        {            
            this.queue.Enqueue(e.ActionToPerform);
            this.ProcessQueue();
        }

        private bool ActionsAreIdentical(ArchivingAction action1, ArchivingAction action2)
        {
            return action1 != null && action2 != null
                && action1.ActionType == action2.ActionType
                && action1.Item == action2.Item
                && action1.SourcePath == action2.SourcePath
                && action1.TargetPath == action2.TargetPath;
        }
    }
}
