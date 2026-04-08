

using MixBrev.Models;

namespace MixBrev.Domain
  
{
    public interface ISyncService    {
        Task<SyncResult> ExecuteFullSyncAsync(string cohortId);
        Task<List<SyncResult>> SyncAllCohortsAsync();
    }
}
