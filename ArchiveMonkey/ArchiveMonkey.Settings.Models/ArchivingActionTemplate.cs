using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchivingActionTemplate : BasePropertyChanged
    {
        private Guid actionId;
        private ArchivingActionType actionType;
        private Guid inputArchiveId;
        private Guid outputArchiveId;
        private Archive inputArchive;
        private Archive outputArchive;

        public ArchivingActionTemplate()
        {
            this.ActionId = Guid.NewGuid();
        }

        [DataMember(IsRequired = true)]
        public Guid ActionId
        {
            get
            {
                return this.actionId;
            }

            set
            {
                if (this.actionId != value)
                {
                    this.actionId = value;
                    this.RaisePropertyChanged("ActionId");
                }
            }
        }

        [DataMember(IsRequired = true)]
        public ArchivingActionType ActionType
        {
            get
            {
                return this.actionType;
            }

            set
            {
                if (this.actionType != value)
                {
                    this.actionType = value;
                    this.RaisePropertyChanged("ActionType");
                }
            }
        }

        [DataMember(IsRequired = true)]
        public Guid InputArchiveId
        {
            get
            {
                return this.inputArchiveId;
            }

            set
            {
                if (this.inputArchiveId != value)
                {
                    this.inputArchiveId = value;
                    this.RaisePropertyChanged("InputArchiveId");
                }
            }
        }

        [DataMember(IsRequired = true)]
        public Guid OutputArchiveId
        {
            get
            {
                return this.outputArchiveId;
            }

            set
            {
                if (this.outputArchiveId != value)
                {
                    this.outputArchiveId = value;
                    this.RaisePropertyChanged("OutputArchiveId");
                }
            }
        }       

        public Archive InputArchive
        {
            get
            {
                return this.inputArchive;
            }

            set
            {
                if (this.inputArchive != value)
                {
                    this.inputArchive = value;
                    this.RaisePropertyChanged("InputArchive");
                    this.inputArchiveId = value != null ? value.ArchiveId : Guid.Empty;
                }
            }
        }

        public Archive OutputArchive
        {
            get
            {
                return this.outputArchive;
            }

            set
            {
                if (this.outputArchive != value)
                {
                    this.outputArchive = value;
                    this.RaisePropertyChanged("OutputArchiveName");
                    this.outputArchiveId = value != null ? value.ArchiveId : Guid.Empty;
                }
            }
        }
    }

    public enum ArchivingActionType
    {
        Copy = 1,
        Move = 2
    }
}
