using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveMonkey.Settings.Helpers;
using ArchiveMonkey.Settings.Models;
using ArchiveMonkey.Settings.Services;

namespace ArchiveMonkey.Settings.ViewModels
{
    public class SettingsViewModel : BasePropertyChanged
    {
        private readonly ISettingsService settingsService;
        private ArchiveMonkeySettings settings;
        private Archive selectedArchive;
        private ArchivingAction selectedAction;
        public IEnumerable<ArchivingActionType> archivingActionTypes;        

        public SettingsViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            this.Settings = settingsService.GetSettings();            

            this.AddArchiveCommand = new SimpleCommand(() => true, this.NewArchive);
            this.DeleteArchiveCommand = new SimpleCommand(this.CanDeleteArchive, this.DeleteArchive);
            this.AddActionCommand = new SimpleCommand(() => true, this.NewAction);
            this.DeleteActionCommand = new SimpleCommand(this.CanDeleteAction, this.DeleteAction);
        }

        public SimpleCommand AddArchiveCommand { get; private set; }
        public SimpleCommand DeleteArchiveCommand { get; private set; }
        public SimpleCommand AddActionCommand { get; private set; }
        public SimpleCommand DeleteActionCommand { get; private set; }        

        public ArchiveMonkeySettings Settings
        {
            get
            {
                return this.settings;
            }

            set
            {
                if(this.settings != value)
                {
                    this.settings = value;
                    this.RaisePropertyChanged("Settings");
                }
            }
        }        

        public Archive SelectedArchive
        {
            get
            {
                return this.selectedArchive;
            }

            set
            {
                if(this.selectedArchive != value)
                {
                    this.selectedArchive = value;
                    this.RaisePropertyChanged("SelectedArchive");
                    this.DeleteArchiveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public ArchivingAction SelectedAction
        {
            get
            {
                return this.selectedAction;
            }

            set
            {
                if(this.selectedAction != value)
                {
                    this.selectedAction = value;
                    this.RaisePropertyChanged("SelectedAction");
                    this.DeleteActionCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public IEnumerable<ArchivingActionType> ArchivingActionTypes
        {
            get
            {
                return this.archivingActionTypes = this.archivingActionTypes ?? new List<ArchivingActionType>(new ArchivingActionType[]
                                                                                    {
                                                                                        ArchivingActionType.Copy,
                                                                                        ArchivingActionType.Move
                                                                                    });
            }
        }

        public void NewArchive()
        {
            var archive = new Archive()
            {
                DisplayName = "harro",
                Path = "Super Path"
            };
            
            this.Settings.Archives.Add(archive);
        }

        public void NewAction()
        {
            var action = new ArchivingAction();            
            this.Settings.ArchivingActions.Add(action);
        }

        public void DeleteArchive()
        {           
            this.Settings.Archives.Remove(this.SelectedArchive);
        }

        public void DeleteAction()
        {   
            this.Settings.ArchivingActions.Remove(this.SelectedAction);
        }

        private bool CanDeleteArchive()
        {
            return this.SelectedArchive != null;
        }

        private bool CanDeleteAction()
        {
            return this.SelectedAction != null;
        }        
    }
}
