using MixBrev.Domain;
using Supabase.Storage;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;
using MixBrev.Models;

namespace MixBrev.Services
{
    public class MixpanelService : IMixpanelService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public MixpanelService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<string>> GetMixpanelDataAsync(string cohortId)
        {
            var distinctIds = new List<string>();

            string secret = _configuration["Mixpanel:Secret"];
            string projectId = _configuration["Mixpanel:ProjectId"];
            string Basic  = _configuration["Mixpanel:Basic"];


            var client = _httpClientFactory.CreateClient();

            // Autenticación Basic
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Basic}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // 1. Cambiamos la URL al endpoint de Query Profiles (engage)
            var url = $"https://mixpanel.com/api/query/engage?project_id={projectId}";

            // 2. Preparamos el cuerpo con el formato JSON que vimos en la web
            var payload = new Dictionary<string, string>
    {
        { "filter_by_cohort", $"{{\"id\": {cohortId}}}" }
    };

            var content = new FormUrlEncodedContent(payload);

            // 3. Cambiamos GET por POST
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return distinctIds;
            }

            // 4. Procesamos la respuesta
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseString);

            // El endpoint de 'engage' devuelve un array en la propiedad "results"
            var results = jsonResponse["results"] as JArray;

            if (results != null)
            {
                foreach (var item in results)
                {
                    var email = item["$properties"]?["$email"]?.ToString();
                    if (!string.IsNullOrEmpty(email))
                    {
                        distinctIds.Add(email);
                    }
                }
            }

            return distinctIds;
        }

        public async Task<List<User>> GetUserMixpanel(string cohortId)
        {
            var distinctIds = new List<User>();

            string secret = _configuration["Mixpanel:Secret"];
            string projectId = _configuration["Mixpanel:ProjectId"];
            string Basic = _configuration["Mixpanel:Basic"];


            var client = _httpClientFactory.CreateClient();

            // Autenticación Basic
            var authToken = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Basic}:{secret}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // 1. Cambiamos la URL al endpoint de Query Profiles (engage)
            var url = $"https://mixpanel.com/api/query/engage?project_id={projectId}";

            // 2. Preparamos el cuerpo con el formato JSON que vimos en la web
            var payload = new Dictionary<string, string>
           {
            { "filter_by_cohort", $"{{\"id\": {cohortId}}}" }
           };

            var content = new FormUrlEncodedContent(payload);

            // 3. Cambiamos GET por POST
            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                return distinctIds;
            }

            // 4. Procesamos la respuesta
            var responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JObject.Parse(responseString);

            // El endpoint de 'engage' devuelve un array en la propiedad "results"
            var results = jsonResponse["results"] as JArray;

            if (results != null && results.Any())
            {
                foreach (var item in results)
                {   
                    var user = new User
                    {
                        email = item["$properties"]?["$email"]?.ToString(),
                        first_name = item["$properties"]?["$first_name"]?.ToString(),
                        last_names = item["$properties"]?["$last_name"]?.ToString(),
                        user_account_id = item["$properties"]?["user_id"]?.ToString(),
                        company_id = item["$properties"]?["company_id"]?.ToString()
                    };

                    distinctIds.Add(user);
                }
            }

          return distinctIds; ;
        }
    }
}
