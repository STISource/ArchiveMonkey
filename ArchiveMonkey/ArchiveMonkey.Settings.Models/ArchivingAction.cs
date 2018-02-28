using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchivingAction : BasePropertyChanged
    {
        private Guid actionId;
        private ArchivingActionType actionType;
        private Guid inputArchiveId;
        private Guid outputArchiveId;

        public ArchivingAction()
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
    }

    public enum ArchivingActionType
    {
        Copy = 1,
        Move = 2
    }
}
