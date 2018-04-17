using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchivingActionTemplate : BasePropertyChanged
    {
        private const int DefaultRetryDelay = 30;
        private const int DefaultRetryCount = 5;

        private Guid actionId;
        private ArchivingActionType actionType;
        private Guid inputArchiveId;
        private Guid outputArchiveId;
        private Archive inputArchive;
        private Archive outputArchive;
        private int? retryCount;
        private int? retryDelay;
        private string filter;

        public ArchivingActionTemplate()
        {
            this.ActionId = Guid.NewGuid();
            this.ActionType = ArchivingActionType.Copy;
            this.RetryCount = DefaultRetryCount;
            this.RetryDelay = DefaultRetryDelay;
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

        [DataMember]
        public int? RetryCount
        {
            get
            {
                return this.retryCount;
            }

            set
            {
                if(this.retryCount != value)
                {
                    if (!this.retryCount.HasValue && value.HasValue)
                    {
                        this.RetryDelay = DefaultRetryDelay;
                    }
                    else if (this.retryCount.HasValue && !value.HasValue)
                    {
                        this.RetryDelay = null;
                    }

                    this.retryCount = value;
                    this.RaisePropertyChanged("RetryCount");
                }
            }
        }

        [DataMember]
        public int? RetryDelay
        {
            get
            {
                return this.retryDelay;
            }

            set
            {
                if (this.retryDelay != value)
                {
                    this.retryDelay = value;
                    this.RaisePropertyChanged("RetryDelay");
                }
            }
        }

        [DataMember]
        public string Filter
        {
            get
            {
                return this.filter;
            }

            set
            {
                if (this.filter != value)
                {
                    this.filter = value;
                    this.RaisePropertyChanged("Filter");
                }
            }
        }
    }

    public enum ArchivingActionType
    {
        Copy = 1        
    }
}
