using Microsoft.AspNetCore.Mvc;
using MixBrev.Domain;
using MixBrev.Services;
using System.Configuration;

namespace MixBrev.Controllers
{

    [ApiController] // Añade esto para que sea un API Controller formal
    [Route("api/[controller]")]
    public class SyncControl : ControllerBase
    {
        private readonly ISyncService _syncService; // Definir la variable
        private readonly IConfiguration _configuration; // Para acceder a la configuración

        // El Constructor: Aquí es donde sucede la "magia" de la inyección
        public SyncControl(ISyncService syncService, IConfiguration configuration)
        {
            _syncService = syncService;
            _configuration = configuration;
        }

        [HttpGet("/api/cron/sync-cohorts")]
        public async Task<IActionResult> RunSync()
        {
            // Vercel envía el secreto en el header "Authorization"
            var authHeader = Request.Headers["Authorization"].ToString();
            var cronSecret = _configuration["CRON_SECRET"]; // Variable que Vercel crea sola

            if (string.IsNullOrEmpty(authHeader) || authHeader != $"Bearer {cronSecret}")
            {
                return Unauthorized();
            }

            // Ahora _syncService ya existe y no marcará error
            var results = await _syncService.SyncAllCohortsAsync();
            return Ok(results);
        }

        [HttpGet("api/single/{cohortId}")]
        public async Task<IActionResult> RunSingleSync(string cohortId)

        {

            if (!Request.Headers.TryGetValue("x-sync-key", out var extractedKey) ||
                extractedKey != _configuration["Sync:SecretKey"])
            {
                return Unauthorized("Clave de sincronización inválida.");
            }
            var result = await _syncService.ExecuteFullSyncAsync(cohortId);

            if (result.TotalFoundInMixpanel == 0)
                return NotFound($"No se encontraron usuarios para la cohorte {cohortId}");

            return Ok(result);
        }




    }
}
