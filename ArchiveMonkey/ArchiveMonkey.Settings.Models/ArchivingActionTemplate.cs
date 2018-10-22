using System;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchivingActionTemplate : BasePropertyChanged
    {
        private const int DefaultRetryDelay = 20;
        private const int DefaultRetryCount = 10;

        private Guid actionId;
        private ArchivingActionType actionType;
        private Guid inputArchiveId;
        private Guid outputArchiveId;
        private Guid outputArchiveAfterDueDateId;
        private Archive inputArchive;
        private Archive outputArchive;
        private Archive outputArchiveAfterDueDate;
        private DateTime? dueDate;
        private int? retryCount;
        private int? retryDelay;
        private string filter;
        private int sequence;
        private bool handleSynchronously;
        private bool includeInternalItems;


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
        
        [DataMember(IsRequired = false)]
        public Guid OutputArchiveAfterDueDateId
        {
            get
            {
                return this.outputArchiveAfterDueDateId;
            }

            set
            {
                if (this.outputArchiveAfterDueDateId != value)
                {
                    this.outputArchiveAfterDueDateId = value;
                    this.RaisePropertyChanged("OutputArchiveAfterDueDateId");
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
                    this.RaisePropertyChanged("OutputArchive");
                    this.outputArchiveId = value != null ? value.ArchiveId : Guid.Empty;
                }
            }
        }

        public Archive OutputArchiveAfterDueDate
        {
            get
            {
                return this.outputArchiveAfterDueDate;
            }

            set
            {
                if (this.outputArchiveAfterDueDate != value)
                {
                    this.outputArchiveAfterDueDate = value;
                    this.RaisePropertyChanged("OutputArchiveAfterDueDate");
                    this.outputArchiveAfterDueDateId = value != null ? value.ArchiveId : Guid.Empty;
                }
            }
        }

        [DataMember(IsRequired = false)]
        public DateTime? DueDate
        {
            get
            {
                return this.dueDate;
            }

            set
            {
                if (this.dueDate != value)
                {
                    this.dueDate = value;
                    this.RaisePropertyChanged("DueDate");                    
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

        [DataMember]
        public int Sequence
        {
            get
            {
                return this.sequence;
            }

            set
            {
                if(this.sequence != value)
                {
                    this.sequence = value;
                    this.RaisePropertyChanged("Sequence");
                }
            }
        }

        [DataMember]
        public bool HandleSynchronously
        {
            get
            {
                return this.handleSynchronously;
            }

            set
            {
                if(this.handleSynchronously != value)
                {
                    this.handleSynchronously = value;
                    this.RaisePropertyChanged("HandleSynchronously");
                }
            }
        }

        [DataMember]
        public bool IncludeInternalItems
        {
            get
            {
                return this.includeInternalItems;
            }

            set
            {
                if(this.includeInternalItems != value)
                {
                    this.includeInternalItems = value;
                    this.RaisePropertyChanged("IncludeInternalItems");
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
