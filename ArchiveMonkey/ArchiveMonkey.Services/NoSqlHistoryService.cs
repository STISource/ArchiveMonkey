using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace ArchiveMonkey.Services
{
    public class NoSqlHistoryService : IArchivingHistoryService
    {
        public const string DatabaseName = @"History.db";
        public const string CollectionName = @"ArchivingHistoryEntries";
        public const int MaxHistoryPerInputArchive = 200;

        public void AddToHistory(HistoryEntry entry)
        {
            using (var database = new LiteDatabase(DatabaseName))
            {
                // Open collection (or create one if not exists)
                var historyEntries = database.GetCollection<HistoryEntry>(CollectionName);
                historyEntries.Insert(entry);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<HistoryEntry> ReadHistory(string sourcePath)
        {
            List<HistoryEntry> results = null;

            // Open database (or create if not exits)
            using (var database = new LiteDatabase(DatabaseName))
            {
                // Open collection (or create one if not exists)
                var historyEntries = database.GetCollection<HistoryEntry>(CollectionName);

                // get list. newest first!
                results = historyEntries.Find(x => x.SourcePath == sourcePath).OrderByDescending(x => x.ArchivingDate).ToList();

                // if there are already more history elements then desired, delete old ones
                if(results.Count > MaxHistoryPerInputArchive)
                {
                    for (int i = results.Count - 1; i >= MaxHistoryPerInputArchive; i--)
                    {
                        var entryToDelete = results[i];

                        historyEntries.Delete(x => x.Id == entryToDelete.Id);
                        results.RemoveAt(i);
                    }
                }                
            }

            return results;
        }
    }
}
