namespace MixBrev.Models
{
    public class SyncResult
    {
        // Cuántos IDs entregó Mixpanel originalmente
        public int TotalFoundInMixpanel { get; set; }

        // Cuántos de esos IDs tenían un perfil real en tu base de datos (Supabase)
        public int EnrichedFromData { get; set; }

        public string CohortId { get; set; }

        // Cuántos se enviaron con éxito a Brevo
        public int SuccessfullySynced { get; set; }

        // Una lista de errores opcional por si quieres saber qué falló sin detener el proceso
        public List<string> Errors { get; set; } = new List<string>();

        // Tiempo que tardó la ejecución
        public double DurationSeconds { get; set; }

        // Fecha de ejecución
        public DateTime ExecutionDate { get; set; } = DateTime.UtcNow;
    }


}

