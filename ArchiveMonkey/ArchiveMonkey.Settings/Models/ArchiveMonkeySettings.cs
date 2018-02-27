using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchiveMonkeySettings
    {
        [DataMember]
        public IList<Archive> Archives { get; set; }

        [DataMember]
        public IList<ArchivingAction> ArchivingActions { get; set; }
    }    
}
