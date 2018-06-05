using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class Archive : BasePropertyChanged
    {
        private Guid archiveId;
        private string baseLocalPath;
        private string baseNetworkPath;
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

        [DataMember]
        public string BaseLocalPath
        {
            get
            {
                return this.baseLocalPath;
            }
            set
            {
                if (this.baseLocalPath != value)
                {
                    this.baseLocalPath = value;
                    this.RaisePropertyChanged("BaseLocalPath");
                    this.RaisePropertyChanged("FullLocalPath");
                }
            }
        }

        [DataMember]
        public string BaseNetworkPath
        {
            get
            {
                return this.baseNetworkPath;
            }
            set
            {
                if (this.baseNetworkPath != value)
                {
                    this.baseNetworkPath = value;
                    this.RaisePropertyChanged("BaseNetworkPath");
                    this.RaisePropertyChanged("FullNetworkPath");
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
                    this.RaisePropertyChanged("FullLocalPath");
                    this.RaisePropertyChanged("FullNetworkPath");
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

        public string FullLocalPath
        {
            get { return string.IsNullOrEmpty(this.BaseLocalPath) ? this.Path : this.BaseLocalPath + this.Path; }
        }

        public string FullNetworkPath
        {
            get { return string.IsNullOrEmpty(this.BaseNetworkPath) ? this.Path : this.BaseNetworkPath + this.Path; }
        }

        public override string ToString()
        {
            return this.DisplayName;
        }
    }
}
