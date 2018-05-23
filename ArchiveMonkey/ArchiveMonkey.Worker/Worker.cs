using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiveMonkey.Services;
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
        private readonly IFilterService filterService;

        public bool Processing { get; private set; }

        public Worker(ArchiveMonkeySettings settings, Queue<ArchivingAction> queue, IArchiver archiver, UnityContainer iocContainer)
        {
            this.settings = settings;
            this.queue = queue;
            this.iocContainer = iocContainer;
            this.archiveWatchers = new List<IArchiveWatcher>();
            this.archiver = archiver;
            this.filterService = iocContainer.Resolve<IFilterService>();
            this.Processing = false;

            foreach (var action in this.settings.ArchivingActionTemplates.OrderBy(x => x.InputArchive.Path).ThenBy(y => y.Sequence))
            {
                // create one queue entry for every archiving action to ensure changes are processed that might have been missed since last program run
                this.queue.Enqueue(ArchivingAction.FromTemplate(action, this.filterService));
            }
        }        

        public void Run()
        {
            logger.Info("Worker starting ...");

            foreach(var actionGroup in this.settings.ArchivingActionTemplates.OrderBy(x => x.InputArchive.Path).GroupBy(x => x.InputArchiveId))
            {
                IArchiveWatcher watcher = null;
                try
                {
                    // add a watcher per source directory. It is possible that there are several action templates with the same source directory
                    watcher = this.iocContainer.Resolve<IArchiveWatcher>();
                    watcher.InputArchiveChanged += WatcherInputArchiveChanged;
                    watcher.Watch(actionGroup);
                    this.archiveWatchers.Add(watcher);
                }
                catch(ArgumentException ex)
                {                    
                    watcher.InputArchiveChanged -= WatcherInputArchiveChanged;
                    logger.Error(ex, "Could not start watcher. " + ex.Message);
                }
            }            

            logger.Info("Worker started.");
            this.ProcessQueue();
        }

        public void StopWatching()
        {
            foreach(var watcher in this.archiveWatchers)
            {
                watcher.Stop();
                logger.Info("Watching folder {0} stopped.", watcher.WatchedArchivingActionTemplates.First().InputArchive.DisplayName);
            }
        }

        private void ProcessQueue()
        {
            // ensure processing occurs only once at a time
            if(this.Processing)
            {
                return;
            }

            this.Processing = true;

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

                if (string.IsNullOrEmpty(action.Item))
                {
                    logger.Info("Start archiving of latest changes synchronously.");
                    this.archiver.Archive(action);
                }
                else
                {
                    if(action.HandleSynchronously)
                    {
                        logger.Info("Start archiving of item synchronously.");
                        this.archiver.Archive(action);
                    }
                    else
                    {
                        logger.Info("Start archiving of item asynchronously.");
                        Task.Run(() => this.archiver.Archive(action));
                    }                    
                }
            }

            logger.Info("Processing worker queue finished.");

            this.Processing = false;
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
