using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ArchiveMonkey.Services;
using ArchiveMonkey.Settings.Models;
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
            logger.Info("Archiving latest changes from {0} - {1} to {2} - {3}", action.SourceArchiveName, action.RelativeSourcePath, action.TargetArchiveName, action.RelativeTargetPath);

            var historyEntries = this.historyService.ReadHistory(action.FullNetworkSourcePath);            

            if(!historyEntries.Any())
            {
                logger.Warn("No history entries found for {0} - {1}", action.SourceArchiveName, action.FullNetworkSourcePath);
                return;
            }

            try
            {                
                var davidAccount = this.ConnectToDavidServer();

                var sourceArchive = davidAccount.GetArchive(action.FullNetworkSourcePath);
                var targetArchive = davidAccount.GetArchive(action.FullNetworkTargetPath);

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

                    if ((mail.IsExternal == true || action.IncludeInternalItems)
                        && mailSendRecievedDate > considerMailsFrom
                        && !historyEntries.Any(x => x.ArchivedItem == mail.TextSource.ToLower()))
                    {
                        logger.Debug("Found mail that has not yet been archived. From {0} To {1} at {2}", mail.From.EMail, mail.Destination, mail.StatusTime);

                        if (action.Filter != null)
                        {
                            try
                            {
                                logger.Info("Applying filter {0}", action.Filter.ToString());
                                if (!action.Filter.FilterApplies(mail))
                                {
                                    logger.Info("Filter does not match.");
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("Could not evaluate filter. Exception: {0}", ex.Message);
                                continue;
                            }

                            logger.Info("Filter matches.");
                        }
                        
                        logger.Info("Copying mail {0} from {1} to {2}", mail.TextSource, action.SourceArchiveName, action.TargetArchiveName);
                        mail.Copy(targetArchive);

                        if(action.ActionType == ArchivingActionType.Move)
                        {
                            logger.Info("Removing mail {0} from {1} as the configured action type is \"move\"", mail.TextSource, action.SourceArchiveName);
                            mail.Delete();
                        }

                        // add history entry
                        this.historyService.AddToHistory(new HistoryEntry
                        {
                            ArchivingDate = DateTime.Now,
                            SourcePath = action.FullNetworkSourcePath,
                            TargetPath = action.FullNetworkTargetPath,
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
            var actionGuid = Guid.NewGuid();
            var fullNetworkPathOfItem = action.Item
                                            .ToLower()
                                            .Replace(action.FullLocalSourcePath.ToLower(), action.FullNetworkSourcePath.ToLower());

            logger.Info("ActionGuid {0}: Archiving of {1} from {2} - {3} to {4} - {5}", actionGuid, fullNetworkPathOfItem, action.SourceArchiveName, action.RelativeSourcePath, action.TargetArchiveName, action.RelativeTargetPath);

            int numberOfRetries = action.RetryCount.HasValue && action.RetryCount.Value > 0 ? action.RetryCount.Value : 0;
            int delay = action.RetryDelay ?? 20;
            var processedIds = new List<int>();            

            for (int tryCount = 0; tryCount <= numberOfRetries; tryCount++)
            {
                logger.Info("ActionGuid {0}: {1}. try ...", actionGuid, tryCount + 1);
                bool retryNeeded = true;
                try
                {
                    var davidAccount = this.ConnectToDavidServer();
                    logger.Debug("ActionGuid {0}: Connected to David server {1}.", actionGuid, this.login.Server);

                    var sourceArchive = davidAccount.GetArchive(action.FullNetworkSourcePath);
                    var targetArchive = davidAccount.GetArchive(action.FullNetworkTargetPath);

                    logger.Info("ActionGuid {0}: Found {1} mail items in source archive.", actionGuid, sourceArchive.MailItems.Count);
                    var mailFound = false;
                    int expectedEntries = 0;

                    for (int i = 0; i < sourceArchive.MailItems.Count; i++)
                    {
                        var item = sourceArchive.MailItems.Item(i);
                        if (!(item is MailItem))
                        {
                            logger.Warn("ActionGuid {0}: Item is not a mail item. Subject: {1}", actionGuid, item.Subject);
                            continue;
                        }

                        var mail = (MailItem)item;
                        logger.Debug("ActionGuid {0}: Testing item {1}: {2}, Mail date: {3}, External: {4}", actionGuid, i, mail.TextSource.ToLower(), mail.StatusTime, mail.IsExternal);

                        if (mail.TextSource.ToLower() != fullNetworkPathOfItem)
                        {
                            logger.Debug("ActionGuid {0}: This is not the right mail.", actionGuid);
                            continue;
                        }

                        logger.Info("ActionGuid {0}: Found right mail. From {1} To {2} at {3}", actionGuid, mail.From.EMail, mail.Destination, mail.StatusTime);

                        // we've found the right mail. But possibly there are more entries in the archive for that same mail in case of outgoing.
                        // that is david style for outgoing mails. Multiple entries just because there are multiple recipients.
                        // As some recipients/entries for that mail might be external and some internal let's not break this iteration hastily.
                        // let's just look how much more entries to expect                        
                        if (!mailFound)
                        {
                            expectedEntries = !mail.Received ? mail.Recipients.Count + mail.CC.Count + mail.BCC.Count : 1;
                            expectedEntries = expectedEntries - processedIds.Count;
                        }
                        mailFound = true;

                        if (processedIds.Contains((int)mail._ID))
                        {
                            logger.Info("ActionGuid {0}: Mail has already been processed in case of multiple outgoing recipients.", actionGuid);
                            continue;
                        }                        
                        
                        expectedEntries--;
                        processedIds.Add((int)mail._ID);

                        if (!mail.IsExternal && !action.IncludeInternalItems)
                        {
                            logger.Info("ActionGuid {0}: Internal mail. No internal mails to be included here. No action taken.", actionGuid);                            
                            if(expectedEntries < 1)
                            {
                                retryNeeded = false;
                                break;
                            }

                            logger.Info("ActionGuid {0}: {1} more recipient mail entries expected.", actionGuid, expectedEntries);
                            continue;
                        }

                        if (action.Filter != null)
                        {
                            try
                            {
                                logger.Info("ActionGuid {0}: Applying filter {1}", actionGuid, action.Filter.ToString());
                                if(!action.Filter.FilterApplies(mail))
                                {
                                    logger.Info("ActionGuid {0}: Filter does not match.", actionGuid);
                                    if (expectedEntries < 1)
                                    {
                                        retryNeeded = false;
                                        break;
                                    }

                                    logger.Info("ActionGuid {0}: {1} more recipient mail entries expected.", actionGuid, expectedEntries);
                                    continue;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error("ActionGuid {0}: Could not evaluate filter. Exception: {1}", actionGuid, ex.Message);
                                if (expectedEntries < 1)
                                {
                                    retryNeeded = false;
                                    break;
                                }

                                logger.Info("ActionGuid {0}: {1} more recipient mail entries expected.", actionGuid, expectedEntries);
                                continue;
                            }

                            logger.Info("ActionGuid {0}: Filter matches.", actionGuid);
                        }

                        logger.Info("ActionGuid {0}: Copying ...", actionGuid);
                        mail.Copy(targetArchive);

                        if (action.ActionType == ArchivingActionType.Move)
                        {
                            logger.Info("ActionGuid {0}: Removing mail {1} from {2} as the configured action type is \"move\"", actionGuid, fullNetworkPathOfItem, action.SourceArchiveName);
                            mail.Delete();
                        }

                        // add history entry
                        this.historyService.AddToHistory(new HistoryEntry
                        {
                            ArchivingDate = DateTime.Now,
                            SourcePath = action.FullNetworkSourcePath,
                            TargetPath = action.FullNetworkTargetPath,
                            ArchivedItem = fullNetworkPathOfItem,
                            AdditionalInfo1 = mail.From.EMail,
                            AdditionalInfo2 = mail.Destination,
                            AdditionalInfo3 = mail.Subject
                        });

                        // if we successfully archived an email there is no further need to iterate through except we expect more entries
                        if (expectedEntries < 1)
                        {
                            retryNeeded = false;
                            break;
                        }

                        logger.Info("ActionGuid {0}: {1} more recipient mail entries expected.", actionGuid, expectedEntries);
                        continue;
                    }

                    davidAccount.Logoff();
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "ActionGuid {0}: {1}\n{2}\n{3}", actionGuid, "Error during archiving of mail item.", ex.Message, ex.StackTrace);
                }

                if(retryNeeded && tryCount < numberOfRetries)
                {
                    logger.Info("ActionGuid {0}: Retry needed.", actionGuid);
                    Thread.Sleep(delay * 1000);
                }
                else
                {
                    break;
                }
            }

            logger.Info("ActionGuid {0}: Archiving of {1} from {2} to {3} finished.", actionGuid, fullNetworkPathOfItem, action.SourceArchiveName, action.TargetArchiveName);
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
