using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public class ArchiveMonkeySettings
    {
        [DataMember(IsRequired = true)]
        public string BasePath { get; set; }

        [DataMember]
        public IList<Archive> DavidArchives { get; set; }

        [DataMember]
        public IList<ArchivingAction> ArchivingActions { get; set; }
    }    
}
