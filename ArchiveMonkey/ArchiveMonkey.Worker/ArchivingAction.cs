using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;
using System;

namespace ArchiveMonkey.Worker
{
    public class ArchivingAction
    {
        public static ArchivingAction FromTemplate(ArchivingActionTemplate template, IFilterService filterService, string item = null)
        {
            var targetArchive = template.DueDate.HasValue && template.DueDate.Value < DateTime.Now && template.OutputArchiveAfterDueDate != null
                                ? template.OutputArchiveAfterDueDate
                                : template.OutputArchive;

            return new ArchivingAction
            {
                ActionType = template.ActionType,
                RelativeSourcePath = template.InputArchive.Path,
                FullLocalSourcePath = template.InputArchive.FullLocalPath,
                FullNetworkSourcePath = template.InputArchive.FullNetworkPath,
                SourceArchiveName = template.InputArchive.DisplayName,
                RelativeTargetPath = targetArchive.Path,
                FullLocalTargetPath = targetArchive.FullLocalPath,
                FullNetworkTargetPath = targetArchive.FullNetworkPath,
                TargetArchiveName = targetArchive.DisplayName,
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
