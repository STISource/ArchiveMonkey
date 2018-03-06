using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Settings.Models
{
    [DataContract]
    public partial class ArchiveMonkeySettings : BasePropertyChanged
    {
        private ObservableCollection<Archive> archives;
        private ObservableCollection<ArchivingActionTemplate> archivingActions;

        public ArchiveMonkeySettings()
        {
            this.Archives = new ObservableCollection<Archive>();
            this.ArchivingActionTemplates = new ObservableCollection<ArchivingActionTemplate>();
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
        public ObservableCollection<ArchivingActionTemplate> ArchivingActionTemplates
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
                    this.RaisePropertyChanged("ArchivingActionTemplates");
                }
            }
        }

        public void ResolveDependencies()
        {
            if(this.ArchivingActionTemplates == null)
            {
                return;
            }

            foreach (var action in this.ArchivingActionTemplates)
            {
                action.InputArchive = this.Archives.SingleOrDefault(x => x.ArchiveId == action.InputArchiveId);
                action.OutputArchive = this.Archives.SingleOrDefault(x => x.ArchiveId == action.OutputArchiveId);
            }
        }
    }    
}
