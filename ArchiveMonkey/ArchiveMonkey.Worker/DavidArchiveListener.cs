using System.Collections.Generic;
using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiveListener : IArchiveListener
    {
        private readonly Queue<ArchivingAction> queue;

        public DavidArchiveListener(Queue<ArchivingAction> queue)
        {
            this.queue = queue;
        }

        public void Listen()
        {
            throw new System.NotImplementedException();
        }
    }
}
