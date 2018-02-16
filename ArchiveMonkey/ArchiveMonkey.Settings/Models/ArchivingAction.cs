using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public class ArchivingAction
    {
        [DataMember(IsRequired = true)]
        public ArchivingActionType ActionType { get; set; }

        [DataMember(IsRequired = true)]
        public Archive InputArchive { get; set; }

        [DataMember(IsRequired = true)]
        public Archive OutputArchive { get; set; }
    }

    public enum ArchivingActionType
    {
        Copy = 1,
        Move = 2
    }
}
