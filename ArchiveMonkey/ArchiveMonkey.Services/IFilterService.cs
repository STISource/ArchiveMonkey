namespace ArchiveMonkey.Services
{
    public interface IFilterService
    {
        bool IsValidFilter(string filter);

        IFilter CreateFilter(string filter);
    }
}
