using System;

namespace ArchiveMonkey.Settings.Models
{
    public partial class ArchivingAction : BasePropertyChanged
    {
        private Archive inputArchive;
        private Archive outputArchive;

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
}
