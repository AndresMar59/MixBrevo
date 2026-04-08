using System.Text.Json.Serialization;

namespace MixBrev.Models
{
    public class PaginatedCompanyResponse
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("last_page")]
        public int LastPage { get; set; }

        [JsonPropertyName("data")]
        public List<Models.User> Data { get; set; }



    }
}
