using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Worker
{
    public class ArchivingAction
    {
        public static ArchivingAction FromTemplate(ArchivingActionTemplate template, IFilterService filterService, string item = null)
        {
            return new ArchivingAction
            {
                ActionType = template.ActionType,
                SourcePath = template.InputArchive.Path,
                SourceArchiveName = template.InputArchive.DisplayName,
                TargetPath = template.OutputArchive.Path,
                TargetArchiveName = template.OutputArchive.DisplayName,
                Item = item,
                RetryCount = template.RetryCount,
                RetryDelay = template.RetryDelay,
                Filter = !string.IsNullOrWhiteSpace(template.Filter) ? filterService.CreateFilter(template.Filter) : null,
                HandleSynchronously = template.HandleSynchronously
            };
        }

        public string SourcePath { get; set; }

        public string SourceArchiveName { get; set; }

        public string TargetPath { get; set; }

        public string TargetArchiveName { get; set; }

        public string Item { get; set; }

        public ArchivingActionType ActionType { get; set; }

        public int? RetryCount { get; set; }

        public int? RetryDelay { get; set; }

        public IFilter Filter { get; set; }

        public bool HandleSynchronously { get; set; }
    }
}
