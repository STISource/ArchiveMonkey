using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Worker
{
    public interface IArchiver
    {
        void Archive(ArchivingAction action);
    }
}
