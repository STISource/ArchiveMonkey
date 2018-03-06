using System;
using System.Linq;
using ArchiveMonkey.Services;
using DvApi32;
using NLog;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiver : IArchiver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private readonly string davidServer = "someservername";        

        private readonly IArchivingHistoryService historyService;

        public DavidArchiver(IArchivingHistoryService historyService)
        {
            this.historyService = historyService;
        }

        public void Archive(ArchivingAction action)
        {
            if(string.IsNullOrEmpty(action.Item))
            {
                this.ArchiveLatestChanges(action);
                return;
            }

            this.ArchiveItem(action);
        } 
        
        private void ArchiveLatestChanges(ArchivingAction action)
        {
            logger.Info("Archiving latest changes from {0} to {1}", action.SourcePath, action.TargetPath);

            var historyEntries = this.historyService.ReadHistory(action.SourcePath);            

            if(!historyEntries.Any())
            {
                logger.Warn("No history entries found for {0}", action.SourcePath);
                return;
            }

            try
            {
                var davidApi = new DavidAPIClass();
                var davidAccount = davidApi.GetAccount(this.davidServer);

                var sourceArchive = davidAccount.GetArchive(action.SourcePath);
                var targetArchive = davidAccount.GetArchive(action.TargetPath);

                var considerMailsFrom = historyEntries.Max(x => x.ArchivingDate).AddDays(-1); // ensure no mail is lost

                for (int i = 0; i < sourceArchive.MailItems.Count; i++)
                {
                    var mail = (MailItem2)sourceArchive.MailItems.Item(i);
                    var mailSendRecievedDate = (DateTime)mail.StatusTime;
                    if (mail.IsExternal == true
                        && mailSendRecievedDate > considerMailsFrom
                        && !historyEntries.Any(x => x.ArchivedItem == mail.TextSource.ToLower()))
                    {
                        logger.Info("Copying mail {0} from {1} to {2}", mail.TextSource, action.SourcePath, action.TargetPath);
                        mail.Copy(targetArchive);

                        // add history entry
                        this.historyService.AddToHistory(new HistoryEntry
                        {
                            ArchivingDate = DateTime.Now,
                            SourcePath = action.SourcePath,
                            TargetPath = action.TargetPath,
                            ArchivedItem = mail.TextSource.ToLower(),
                            AdditionalInfo1 = mail.From.EMail,
                            AdditionalInfo2 = mail.Destination,
                            AdditionalInfo3 = mail.Subject
                        });                        
                    }
                }

                davidAccount.Logoff();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error during archiving of latest changes.");
            }

            logger.Info("Archiving latest changes from {0} to {1} finished.", action.SourcePath, action.TargetPath);
        }

        private void ArchiveItem(ArchivingAction action)
        {
            logger.Info("Archiving of {0} from {1} to {2}", action.Item, action.SourcePath, action.TargetPath);

            try
            {
                var davidApi = new DavidAPIClass();                               
                var davidAccount = davidApi.GetAccount(this.davidServer);

                var sourceArchive = davidAccount.GetArchive(action.SourcePath);
                var targetArchive = davidAccount.GetArchive(action.TargetPath);

                for (int i = 0; i < sourceArchive.MailItems.Count; i++)
                {
                    var mail = (MailItem2)sourceArchive.MailItems.Item(i);
                    if (mail.TextSource.ToLower() == action.Item.ToLower()
                        && mail.IsExternal == true)
                    {
                        mail.Copy(targetArchive);

                        // add history entry
                        this.historyService.AddToHistory(new HistoryEntry
                                                            {
                                                                ArchivingDate = DateTime.Now,
                                                                SourcePath = action.SourcePath,
                                                                TargetPath = action.TargetPath,
                                                                ArchivedItem = action.Item.ToLower(),
                                                                AdditionalInfo1 = mail.From.EMail,
                                                                AdditionalInfo2 = mail.Destination,
                                                                AdditionalInfo3 = mail.Subject
                        });
                        break;
                    }
                }

                davidAccount.Logoff();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Error during archiving of mail item.");
            }

            logger.Info("Archiving of {0} from {1} to {2} finished.", action.Item, action.SourcePath, action.TargetPath);
        }
    }
}
