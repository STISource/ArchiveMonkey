namespace ArchiveMonkey.Services
{
    public interface IFilter
    {
        bool FilterApplies(object itemToArchive);
    }
}
