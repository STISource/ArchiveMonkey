using System.Collections.ObjectModel;
using System.Linq;
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

        public void ResolveDependencies()
        {
            foreach (var action in this.ArchivingActions)
            {
                action.InputArchive = this.Archives.Single(x => x.ArchiveId == action.InputArchiveId);
                action.OutputArchive = this.Archives.Single(x => x.ArchiveId == action.OutputArchiveId);
            }
        }
    }    
}
