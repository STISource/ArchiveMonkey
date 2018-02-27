using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class Archive
    {
        public Archive()
        {
            this.ArchiveId = Guid.NewGuid();
        }

        [DataMember(IsRequired = true)]
        public Guid ArchiveId { get; set; }

        [DataMember(IsRequired = true)]
        public string UncPath { get; set; }

        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }
    }
}
