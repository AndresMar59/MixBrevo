using Microsoft.AspNetCore.Mvc;
using MixBrev.Domain;
using MixBrev.Services;
using System.Configuration;

namespace MixBrev.Controllers
{

    [ApiController] // Añade esto para que sea un API Controller formal
    [Route("api/cron")]
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



        [HttpPost("/")]

        public string Post()
        {
            return "Microservidor-vivo. ES NECESARIO LLAVE PARA APIS";
        }

        [HttpGet("sync-cohorts")]
        public IActionResult RunSync([FromServices] IServiceScopeFactory scopeFactory)
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var cronSecret = _configuration["CRON_SECRET"];

            if (string.IsNullOrEmpty(authHeader) || authHeader != $"Bearer {cronSecret}")
            {
                return Unauthorized("Clave de sincronización inválida.");
            }

            // DISPARAR Y OLVIDAR (Correctamente en Railway)
            _ = Task.Run(async () => {
                // Creamos un scope nuevo que no depende de la petición HTTP
                using (var scope = scopeFactory.CreateScope())
                {
                    try
                    {
                        // Obtenemos el servicio desde este nuevo scope
                        var syncService = scope.ServiceProvider.GetRequiredService<ISyncService>();

                        Console.WriteLine($"[{DateTime.Now}] Iniciando sincronización pesada...");
                        await syncService.SyncAllCohortsAsync();
                        Console.WriteLine($"[{DateTime.Now}] Sincronización completada con éxito.");
                    }
                    catch (Exception ex)
                    {
                        // Esto saldrá en tus logs de Railway
                        Console.WriteLine($"[{DateTime.Now}] ERROR en segundo plano: {ex.Message}");
                        if (ex.InnerException != null)
                            Console.WriteLine($"Inner: {ex.InnerException.Message}");
                    }
                }
            });

            return Accepted(new { message = "Proceso iniciado en segundo plano" });
        }

        [HttpGet("{cohortId}")]
        public async Task<IActionResult> RunSingleSync(string cohortId)

        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var cronSecret = _configuration["CRON_SECRET"]; // Variable que Vercel crea sola

            if (string.IsNullOrEmpty(authHeader) || authHeader != $"Bearer {cronSecret}")
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
