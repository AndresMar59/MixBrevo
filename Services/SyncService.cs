using System.Xml.Linq;
using MixBrev.Domain;
using MixBrev.Models;

namespace MixBrev.Services
{
    public class SyncService : ISyncService
    {
        protected readonly IMixpanelService _mixpanel;
        protected readonly IDataMicro _idata;
        protected readonly IBrevoService _brevo;

        public SyncService(IMixpanelService Iservice, IDataMicro Idata, IBrevoService IBrevo) {

            _mixpanel = Iservice;
            _idata = Idata;
            _brevo = IBrevo;
           
        }

        public async Task<SyncResult> ExecuteFullSyncAsync(string cohortId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = new SyncResult { CohortId = cohortId };

            // 1. Mixpanel (Trae todos los IDs de un golpe)
            var ids = await _mixpanel.GetMixpanelDataAsync(cohortId);
            result.TotalFoundInMixpanel = ids.Count;
            if (ids.Count == 0) return result;

            // 2. MicroData (Trae los detalles de un golpe)
            var users = await _idata.GetUsersDetailsAsync(ids);
            result.EnrichedFromData = users.Count;
            if (users.Count == 0) return result;

            // 1. Variable local para el conteo seguro entre hilos
            int totalSuccess = 0;

            // 3. Brevo - PARALELISMO CONTROLADO
            int batchSize = 10;
            for (int i = 0; i < users.Count; i += batchSize)
            {
                var batch = users.Skip(i).Take(batchSize);

                var tasks = batch.Select(async user =>
                {
                    try
                    {
                        var success = await _brevo.SyncUserToBrevoAsync(user, cohortId);
                        if (success)
                        {
                            // AHORA SÍ: Usamos la variable local con ref
                            Interlocked.Increment(ref totalSuccess);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (result.Errors) { result.Errors.Add($"Error {user.email}: {ex.Message}"); }
                    }
                });

                await Task.WhenAll(tasks);
            }

            // --- ¡ESTA LÍNEA ES VITAL! ---
            result.SuccessfullySynced = totalSuccess;
            // ----------------------------

            watch.Stop();
            result.DurationSeconds = watch.Elapsed.TotalSeconds;
            return result;
        }


        public async Task<List<SyncResult>> SyncAllCohortsAsync()
        {
            var cohorts = new List<string>
    {
        "6027991", "6032434", "6028090", "6032435", "6032438", "6032445"
    };

            var overallResults = new List<SyncResult>();

            foreach (var cohortId in cohorts)
            {
                Console.WriteLine($"[{DateTime.Now}] Procesando Cohort: {cohortId}");

                var result = await ExecuteFullSyncAsyncMixpanel(cohortId);
                overallResults.Add(result);

                // Con 1 segundo es suficiente si el proceso interno es eficiente
                await Task.Delay(1000);
            }

            return overallResults;
        }

        public async Task<SyncResult> ExecuteFullSyncAsyncMixpanel(string cohortId)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            var result = new SyncResult { CohortId = cohortId };

            // 1. Mixpanel (Trae todos los IDs de un golpe)
            var users = await _mixpanel.GetUserMixpanel(cohortId);
            result.TotalFoundInMixpanel = users.Count;
            if (users.Count == 0) return result;

            // 1. Variable local para el conteo seguro entre hilos
            int totalSuccess = 0;

            // 3. Brevo - PARALELISMO CONTROLADO
            int batchSize = 10;
            for (int i = 0; i < users.Count; i += batchSize)
            {
                var batch = users.Skip(i).Take(batchSize);

                var tasks = batch.Select(async user =>
                {
                    try
                    {
                        var success = await _brevo.SyncUserToBrevoAsync(user, cohortId);
                        if (success)
                        {
                            // AHORA SÍ: Usamos la variable local con ref
                            Interlocked.Increment(ref totalSuccess);
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (result.Errors) { result.Errors.Add($"Error {user.email}: {ex.Message}"); }
                    }
                });

                await Task.WhenAll(tasks);
            }

            // --- ¡ESTA LÍNEA ES VITAL! ---
            result.SuccessfullySynced = totalSuccess;
            // ----------------------------

            watch.Stop();
            result.DurationSeconds = watch.Elapsed.TotalSeconds;
            return result;
        }

    }
}



    








