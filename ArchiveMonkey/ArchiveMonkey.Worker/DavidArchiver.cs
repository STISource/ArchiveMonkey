using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArchiveMonkey.Services;
using DvApi32;
using NLog;

namespace ArchiveMonkey.Worker
{
    public class DavidArchiver : IArchiver
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();              

        private readonly IArchivingHistoryService historyService;
        private readonly ArchivingLogin login;

        public DavidArchiver(IArchivingHistoryService historyService, ArchivingLogin login)
        {
            this.historyService = historyService;
            this.login = login;
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
            logger.Info("Archiving latest changes from {0} - {1} to {2} - {3}", action.SourceArchiveName, action.SourcePath, action.TargetArchiveName, action.TargetPath);

            var historyEntries = this.historyService.ReadHistory(action.SourcePath);            

            if(!historyEntries.Any())
            {
                logger.Warn("No history entries found for {0}", action.SourceArchiveName);
                return;
            }

            try
            {                
                var davidAccount = this.ConnectToDavidServer();

                var sourceArchive = davidAccount.GetArchive(action.SourcePath);
                var targetArchive = davidAccount.GetArchive(action.TargetPath);

                var considerMailsFrom = historyEntries.Max(x => x.ArchivingDate).AddDays(-1); // ensure no mail is lost

                for (int i = 0; i < sourceArchive.MailItems.Count; i++)
                {
                    var item = sourceArchive.MailItems.Item(i);
                    if(!(item is MailItem))
                    {
                        logger.Warn("Item is not a mail item. Subject: {0}", item.Subject);
                        continue;
                    }

                    var mail = (MailItem)item;
                    var mailSendRecievedDate = (DateTime)mail.StatusTime;

                    logger.Debug("Testing item {0}: {1}, Mail date: {2}, External: {2}", i, mail.TextSource.ToLower(), mail.StatusTime, mail.IsExternal);

                    if (mail.IsExternal == true
                        && mailSendRecievedDate > considerMailsFrom
                        && !historyEntries.Any(x => x.ArchivedItem == mail.TextSource.ToLower()))
                    {
                        logger.Debug("Found external mail that has not yet been archived. From {0} To {1} at {2}", mail.From.EMail, mail.Destination, mail.StatusTime);
                        logger.Info("Copying mail {0} from {1} to {2}", mail.TextSource, action.SourceArchiveName, action.TargetArchiveName);
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
                logger.Error(ex, "{0}\n{1}\n{2}", "Error during archiving of latest changes.", ex.Message, ex.StackTrace);
            }

            logger.Info("Archiving latest changes from {0} to {1} finished.", action.SourceArchiveName, action.TargetArchiveName);
        }

        private void ArchiveItem(ArchivingAction action)
        {
            logger.Info("Archiving of {0} from {1} - {2} to {3} - {4}", action.Item, action.SourceArchiveName, action.SourcePath, action.TargetArchiveName, action.TargetPath);

            int numberOfRetries = action.RetryCount.HasValue && action.RetryCount.Value > 0 ? action.RetryCount.Value : 0;
            int delay = action.RetryDelay ?? 20;

            for (int tryCount = 0; tryCount <= numberOfRetries; tryCount++)
            {
                logger.Debug("{0}. try ...", tryCount + 1);
                bool retryNeeded = true;
                try
                {
                    var davidAccount = this.ConnectToDavidServer();
                    logger.Debug("Connected to David server {0}.", this.login.Server);

                    var sourceArchive = davidAccount.GetArchive(action.SourcePath);
                    var targetArchive = davidAccount.GetArchive(action.TargetPath);

                    logger.Debug("Found {0} mail items in source archive.", sourceArchive.MailItems.Count);

                    for (int i = 0; i < sourceArchive.MailItems.Count; i++)
                    {
                        var item = sourceArchive.MailItems.Item(i);
                        if (!(item is MailItem))
                        {
                            logger.Warn("Item is not a mail item. Subject: {0}", item.Subject);
                            continue;
                        }

                        var mail = (MailItem)item;
                        logger.Debug("Testing item {0}: {1}, Mail date: {2}, External: {3}", i, mail.TextSource.ToLower(), mail.StatusTime, mail.IsExternal);

                        if (mail.TextSource.ToLower() == action.Item.ToLower())
                        {
                            retryNeeded = false;
                            logger.Info("Found right mail. From {0} To {1} at {2}", mail.From.EMail, mail.Destination, mail.StatusTime);

                            if (mail.IsExternal)
                            {
                                logger.Debug("Copying ...");
                                mail.Copy(targetArchive);
                                retryNeeded = false;                                

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
                            }
                            else
                            {
                                logger.Info("Internal mail. No action taken.");
                            }

                            break;
                        }
                    }

                    davidAccount.Logoff();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "{0}\n{1}\n{2}", "Error during archiving of mail item.", ex.Message, ex.StackTrace);
                }

                if(retryNeeded && tryCount < numberOfRetries)
                {
                    Thread.Sleep(delay * 1000);
                }
                else
                {
                    break;
                }
            }

            logger.Info("Archiving of {0} from {1} to {2} finished.", action.Item, action.SourceArchiveName, action.TargetArchiveName);
        }

        private Account ConnectToDavidServer()
        {
            var davidApi = new DavidAPIClass();
            Account davidAccount = null;

            if(string.IsNullOrEmpty(this.login.Username)
                || string.IsNullOrEmpty(this.login.Password))
            {
                davidAccount = davidApi.GetAccount(this.login.Server);
            }
            else
            {
                davidAccount = davidApi.GetAccount(this.login.Server, this.login.Username, this.login.Password);
            }

            return davidAccount;
        }
    }
}
