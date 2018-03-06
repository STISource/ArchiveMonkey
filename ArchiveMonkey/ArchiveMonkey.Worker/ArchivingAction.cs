using ArchiveMonkey.Settings.Models;

namespace ArchiveMonkey.Worker
{
    public class ArchivingAction
    {
        public static ArchivingAction FromTemplate(ArchivingActionTemplate template, string item = null)
        {
            return new ArchivingAction
                        {
                            ActionType = template.ActionType,
                            SourcePath = template.InputArchive.Path,
                            TargetPath = template.OutputArchive.Path,
                            Item = item
                        };
        }

        public string SourcePath { get; set; }

        public string TargetPath { get; set; }

        public string Item { get; set; }

        public ArchivingActionType ActionType { get; set; }
    }
}
