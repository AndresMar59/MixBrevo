using System.Text.Json.Serialization;

namespace MixBrev.Models
{
    public class User
    {
        [JsonPropertyName("user_account_id")]
        public string user_account_id { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("first_name")]
        public string first_name { get; set; }

        [JsonPropertyName("last_names")]
        public string? last_names { get; set; }

        [JsonPropertyName("company_id")]
        public string company_id { get; set; }
    }
}
