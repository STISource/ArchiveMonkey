using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;
using System.Collections.Generic;
using Unity;

namespace ArchiveMonkey.Worker
{
    public class Worker
    {
        private readonly ArchiveMonkeySettings settings;
        private readonly Queue<ArchivingAction> actionQueue;
        private readonly UnityContainer iocContainer;        
        
        public Worker(ArchiveMonkeySettings settings, Queue<ArchivingAction> queue, UnityContainer container)
        {
            this.settings = settings;
            this.actionQueue = queue;
            this.iocContainer = container;
            this.ArchiveListeners = new List<IArchiveListener>();

            foreach (var action in this.settings.ArchivingActions)
            {
                // create one queue entry for every archiving action to ensure changes are processed that might have been missed since last program run
                this.actionQueue.Enqueue(action);

                this.ArchiveListeners.Add(this.iocContainer.Resolve<IArchiveListener>());
            }
        }

        public ICollection<IArchiveListener> ArchiveListeners { get; set; }        

        public void Run()
        {

        }
    }
}
