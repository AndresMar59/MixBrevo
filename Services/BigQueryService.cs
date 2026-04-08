using Google.Cloud.BigQuery.V2;
using MixBrev.Domain;
using MixBrev.Models;

namespace MixBrev.Services
{
    public class BigQueryService : IBigQueryService
    {
        private readonly BigQueryClient _client;
        private readonly string _datasetId = "marketing_sync";
        private readonly string _tableId = "sync_logs";

        public BigQueryService(IConfiguration configuration)
        {
            var projectId = configuration["GoogleCloud:ProjectId"];
            // Nota: Para Vercel, aquí deberías cargar la credencial desde un string de Env Var
            _client = BigQueryClient.Create(projectId);
        }

        public async Task LogSyncEventAsync(UserAccount user, string status)
        {
            var row = new BigQueryInsertRow
        {
            { "external_id", user.Id },
            { "email", user.Email },
            { "company_id", user.Company },
            { "status", status },
            { "timestamp", DateTime.UtcNow } // BigQuery acepta DateTime directamente
        };

            // InsertRowAsync es pesado si se hace 1 por 1 en un bucle grande.
            await _client.InsertRowAsync(_datasetId, _tableId, row);
        }
    }
}
