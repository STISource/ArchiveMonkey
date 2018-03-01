using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchiveMonkeySettings : BasePropertyChanged
    {
        private ObservableCollection<Archive> archives;
        private ObservableCollection<ArchivingAction> archivingActions;

        public ArchiveMonkeySettings()
        {
            this.Archives = new ObservableCollection<Archive>();
            this.ArchivingActions = new ObservableCollection<ArchivingAction>();
        }

        [DataMember]
        public ObservableCollection<Archive> Archives
        {
            get
            {
                return this.archives;
            }

            set
            {
                if (this.archives != value)
                {
                    this.archives = value;
                    this.RaisePropertyChanged("Archives");
                }
            }
        }

        [DataMember]
        public ObservableCollection<ArchivingAction> ArchivingActions
        {
            get
            {
                return this.archivingActions;
            }

            set
            {
                if (this.archivingActions != value)
                {
                    this.archivingActions = value;
                    this.RaisePropertyChanged("ArchivingActions");
                }
            }
        }
    }    
}
