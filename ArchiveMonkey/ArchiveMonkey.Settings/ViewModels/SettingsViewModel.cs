﻿using System;
using System.Collections.Generic;
using System.Linq;
using ArchiveMonkey.Settings.Helpers;
using ArchiveMonkey.Settings.Models;
using ArchiveMonkey.Settings.Properties;
using ArchiveMonkey.Services;
using System.IO;

namespace ArchiveMonkey.Settings.ViewModels
{
    public class SettingsViewModel : BasePropertyChanged
    {
        private readonly ISettingsService settingsService;
        private readonly IFilterService filterService;
        private ArchiveMonkeySettings settings;
        private Archive selectedArchive;
        private ArchivingActionTemplate selectedAction;
        public IEnumerable<ArchivingActionType> archivingActionTypes;
        private string validationErrorMessage;
        private bool validationError;

        public SettingsViewModel(ISettingsService settingsService, IFilterService filterService)
        {
            this.settingsService = settingsService;
            this.filterService = filterService;
            this.Settings = settingsService.GetSettings();
            this.Settings.ResolveDependencies();

            this.AddArchiveCommand = new SimpleCommand(() => true, this.NewArchive);
            this.DeleteArchiveCommand = new SimpleCommand(() => this.SelectedArchive != null, this.DeleteArchive);
            this.AddActionCommand = new SimpleCommand(() => true, this.NewAction);
            this.DeleteActionCommand = new SimpleCommand(() => this.SelectedAction != null, this.DeleteAction);
            this.SaveSettingsCommand = new SimpleCommand(() => true, this.SaveSettings);
        }

        public SimpleCommand AddArchiveCommand { get; private set; }
        public SimpleCommand DeleteArchiveCommand { get; private set; }
        public SimpleCommand AddActionCommand { get; private set; }
        public SimpleCommand DeleteActionCommand { get; private set; }
        public SimpleCommand SaveSettingsCommand { get; private set; }

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

        public ArchivingActionTemplate SelectedAction
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

        public string ValidationErrorMessage
        {
            get
            {
                return this.validationErrorMessage;
            }

            set
            {
                if (this.validationErrorMessage != value)
                {
                    this.validationErrorMessage = value;
                    this.RaisePropertyChanged("ValidationErrorMessage");                    
                }
            }
        }

        public bool ValidationError
        {
            get
            {
                return this.validationError;
            }

            set
            {
                if (this.validationError != value)
                {
                    this.validationError = value;
                    this.RaisePropertyChanged("ValidationError");
                }
            }
        }

        public void NewArchive()
        {
            var archive = new Archive();
            archive.BaseLocalPath = settings.BaseLocalPath;
            archive.BaseNetworkPath = settings.BaseNetworkPath;
            this.Settings.Archives.Add(archive);
            this.SelectedArchive = archive;
        }

        public void NewAction()
        {
            var action = new ArchivingActionTemplate();            
            this.Settings.ArchivingActionTemplates.Add(action);
            this.SelectedAction = action;
        }

        public void DeleteArchive()
        {           
            this.Settings.Archives.Remove(this.SelectedArchive);
        }

        public void DeleteAction()
        {   
            this.Settings.ArchivingActionTemplates.Remove(this.SelectedAction);
        }

        public void SaveSettings()
        {
            this.ValidateSettings();

            if (!this.ValidationError)
            {
                this.settingsService.SaveSettings(this.settings);
            }            
        } 
        
        private void ValidateSettings()
        {
            this.ValidationError = false;

            foreach(var archive in this.settings.Archives)
            {
                if(!Directory.Exists(archive.FullLocalPath)
                    || !Directory.Exists(archive.FullNetworkPath))
                {
                    this.ValidationError = true;
                    this.ValidationErrorMessage = Resources.Settings_Validation_PathDoesNotExist;

                    break;
                }
            }

            if(this.ValidationError)
            {
                return;
            }

            // validations
            foreach (var action in this.settings.ArchivingActionTemplates)
            {
                if (action.InputArchiveId == Guid.Empty
                    || action.OutputArchiveId == Guid.Empty
                    || (action.ActionType != ArchivingActionType.Copy &&  action.ActionType != ArchivingActionType.Move))
                {
                    this.ValidationError = true;
                    this.ValidationErrorMessage = Resources.Settings_Validation_ActionDefinitionIncomplete;
                    break;
                }

                this.ValidationError = !this.ValidationError ? !this.settings.Archives.Any(x => x.ArchiveId == action.InputArchiveId) : true;
                this.ValidationError = !this.ValidationError ? !this.settings.Archives.Any(x => x.ArchiveId == action.OutputArchiveId) : true;

                if(this.ValidationError)
                {
                    this.ValidationErrorMessage = Resources.Settings_Validation_ActionRefersToDeletedArchive;
                    break;
                }

                this.ValidationError = (action.RetryCount.HasValue && action.RetryCount.Value > 0) && !action.RetryDelay.HasValue;
                if(this.ValidationError)
                {
                    this.ValidationErrorMessage = Resources.Settings_Validation_RetryCountNeedsRetryDelay;
                    break;
                }

                this.ValidationError = !string.IsNullOrWhiteSpace(action.Filter) && !this.filterService.IsValidFilter(action.Filter);

                if(this.validationError)
                {
                    this.ValidationErrorMessage = Resources.Settings_Validation_FilterInvalid;
                }
            }            
        }        
    }
}
