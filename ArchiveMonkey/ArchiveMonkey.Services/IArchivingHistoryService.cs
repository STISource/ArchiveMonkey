using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ArchiveMonkey.Services
{
    public interface IArchivingHistoryService
    {
        void AddToHistory(HistoryEntry entry);

        /// <summary>
        /// Returns history entries. Newest first.
        /// </summary>
        /// <param name="sourcePath">Archive to return history entries for</param>
        /// <returns>enumeration of history entries.</returns>
        IEnumerable<HistoryEntry> ReadHistory(string sourcePath);
    }
        
    public class HistoryEntry
    {   
        public int Id { get; set; }

        public DateTime ArchivingDate { get; set; }

        public string SourcePath { get; set; }

        public string TargetPath { get; set; }

        public string ArchivedItem { get; set; }

        public string AdditionalInfo1 { get; set; }

        public string AdditionalInfo2 { get; set; }

        public string AdditionalInfo3 { get; set; }
    }
}
