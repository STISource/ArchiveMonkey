using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public class Archive
    {
        [DataMember(IsRequired = true)]
        public ArchiveUsage Usage { get; set; }

        [DataMember(IsRequired = true)]
        public string UncPath { get; set; }

        [DataMember(IsRequired = true)]
        public string DisplayName { get; set; }
    }

    public enum ArchiveUsage
    {
        Input = 1,
        Output = 2
    }
}
