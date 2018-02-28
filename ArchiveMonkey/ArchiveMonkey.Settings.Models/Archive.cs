using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class Archive : BasePropertyChanged
    {
        private Guid archiveId;
        private string path;
        private string displayName;

        public Archive()
        {
            this.ArchiveId = Guid.NewGuid();
        }

        [DataMember(IsRequired = true)]
        public Guid ArchiveId
        {
            get
            {
                return this.archiveId;
            }

            set
            {
                if(this.archiveId != value)
                {
                    this.archiveId = value;
                    this.RaisePropertyChanged("ArchiveId");
                }
            }
        }

        [DataMember(IsRequired = true)]
        public string Path
        {
            get
            {
                return this.path;
            }

            set
            {
                if (this.path != value)
                {
                    this.path = value;
                    this.RaisePropertyChanged("Path");
                }
            }
        }

        [DataMember(IsRequired = true)]
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }

            set
            {
                if (this.displayName != value)
                {
                    this.displayName = value;
                    this.RaisePropertyChanged("DisplayName");
                }
            }
        }
    }
}
