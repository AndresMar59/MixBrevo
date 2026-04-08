namespace MixBrev.Models
{
    public class SyncData
    {

        public string ExternalId { get; set; }
        public string Email { get; set; }
        public string CompanyId { get; set; }
        public string Status { get; set; } // Ejemplo: "Success", "Failed", "UserNotFound"
        public DateTime Timestamp { get; set; }
        public string Platform { get; set; } = "Brevo";
    }
}
