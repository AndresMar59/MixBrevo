using MixBrev.Models;

namespace MixBrev.Domain
{
    public interface IBigQueryService
    {

        // Registra el evento en el Data Warehouse
        Task LogSyncEventAsync(UserAccount user, string status);

    }
}
