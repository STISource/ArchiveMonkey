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
                RelativeSourcePath = template.InputArchive.Path,
                FullLocalSourcePath = template.InputArchive.FullLocalPath,
                FullNetworkSourcePath = template.InputArchive.FullNetworkPath,
                SourceArchiveName = template.InputArchive.DisplayName,
                RelativeTargetPath = template.OutputArchive.Path,
                FullLocalTargetPath = template.OutputArchive.FullLocalPath,
                FullNetworkTargetPath = template.OutputArchive.FullNetworkPath,
                TargetArchiveName = template.OutputArchive.DisplayName,
                Item = item,
                RetryCount = template.RetryCount,
                RetryDelay = template.RetryDelay,
                Filter = !string.IsNullOrWhiteSpace(template.Filter) ? filterService.CreateFilter(template.Filter) : null,
                HandleSynchronously = template.HandleSynchronously,
                IncludeInternalItems = template.IncludeInternalItems
            };
        }

        public string RelativeSourcePath { get; set; }

        public string FullLocalSourcePath { get; set; }

        public string FullNetworkSourcePath { get; set; }

        public string SourceArchiveName { get; set; }

        public string RelativeTargetPath { get; set; }

        public string FullLocalTargetPath { get; set; }

        public string FullNetworkTargetPath { get; set; }

        public string TargetArchiveName { get; set; }

        public string Item { get; set; }

        public ArchivingActionType ActionType { get; set; }

        public int? RetryCount { get; set; }

        public int? RetryDelay { get; set; }

        public IFilter Filter { get; set; }

        public bool HandleSynchronously { get; set; }

        public bool IncludeInternalItems { get; set; }
    }
}
