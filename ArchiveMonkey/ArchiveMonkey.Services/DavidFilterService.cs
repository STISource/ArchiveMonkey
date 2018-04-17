namespace ArchiveMonkey.Services
{
    public class DavidFilterService : IFilterService
    {
        public IFilter CreateFilter(string filter)
        {
            return new DavidEmailFilter(filter);
        }

        public bool IsValidFilter(string filter)
        {
            return DavidEmailFilter.IsValidFilter(filter);
        }
    }
}
