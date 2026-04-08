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
            var result = new SyncResult();


            result.CohortId = cohortId;

            // 1. Mixpanel
            var ids = await _mixpanel.GetMixpanelDataAsync(cohortId);
            result.TotalFoundInMixpanel = ids.Count;

            if (ids.Count == 0) return result;

            // 2. MicroData
            var users = await _idata.GetUsersDetailsAsync(ids);
            result.EnrichedFromData = users.Count;
            if (users.Count == 0) return result;

            // 3. Brevo 
            foreach (var user in users)
            {
                try
                {
                    var success = await _brevo.SyncUserToBrevoAsync(user, cohortId);
                 // No integrar aun   await _bigQuery.LogSyncEventAsync(user, success ? "Success" : "Brevo_Error");

                    if (success) result.SuccessfullySynced++;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Error syncing {user.email}: {ex.Message}");
                }
            }

            watch.Stop();
            result.DurationSeconds = watch.Elapsed.TotalSeconds;
            return result;
        }

       public async Task<List<SyncResult>> SyncAllCohortsAsync()
        {
            // 1. Definimos la lista de todos tus IDs de cohortes de Mixpanel
            var cohorts = new List<string>
    {
                "6027991" , // Sign-up - Prompt 
                "6032434" , // Cohort Matching-Job
                "6028090" , // Cohort Prompt-Matching
                "6032435" , // Cohort Job-Pipeline
                "6032438" , // Cohort Pipeline-Hire
                "6032445" , // Cohort First-Hire
             //   "6039050"  // Test
    }; 

            var overallResults = new List<SyncResult>();

            // 2. Ejecutamos el ciclo
            foreach (var cohortId in cohorts)
            {
                // Reutilizamos la lógica que ya tienes programada
                var result = await ExecuteFullSyncAsync(cohortId);
                overallResults.Add(result);

                // Esperar 2 segundos entre cohorts para liberar la carga de la API
                await Task.Delay(6000);
            }

            return overallResults;
        }
    }
}
