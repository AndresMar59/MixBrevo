using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MixBrev.Domain;
using MixBrev.Models;
using Newtonsoft.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Supabase.Gotrue;

namespace MixBrev.Services


{
    public class DataMicro : IDataMicro
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public DataMicro(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<List<Models.User>> GetUsersDetailsAsync(List<string> distinctIds)
        {   
            if (distinctIds == null || !distinctIds.Any()) return new List<Models.User>();

            string token = _configuration["Data:Token"];

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("x-api-key", token);

            // 1. Cambiamos la URL al endpoint de Query Profiles (engage)
            var url = "https://data.jobbi.me/data/paginate-company";

            // Definimos los parámetros de paginación
            int page = 1;
            int pageSize = 50; // Cuántos registros queremos por página
            bool hasMoreData = true;

            // Dentro de tu método async...
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var allDistinctIds = new List<Models.User>(); // Tu lista final de strings

            while (hasMoreData)
            {
                // 1. Preparamos el cuerpo de la petición (JSON)

                // 1. Construimos la URL con los parámetros de paginación
                // Esto genera algo como: https://api.com/data?page=1&limit=50
                var urlConParametros = $"{url}?page={page}&page_size={pageSize}";

                var response = await client.GetAsync(urlConParametros);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();

                    // Deserializamos al objeto de paginación
                    var result = System.Text.Json.JsonSerializer.Deserialize<PaginatedCompanyResponse>(jsonResponse, options);

                    if (result?.Data != null && result.Data.Any())
                    {
                        // Sacamos los IDs de esta página y los sumamos a la lista global
                        // Usamos UserAccountId o CompanyId según lo que necesites
                        //  var pageIds = result.Data.Select(x => x.id).ToList();

                        allDistinctIds.AddRange(result.Data);

                        // Lógica para el bucle: ¿hay más páginas?
                        hasMoreData = result.CurrentPage < result.LastPage;
                        page++;
                    }
                    else
                    {
                        hasMoreData = false;
                    }
                }
                else
                {
                    // Si falla una petición (ej. error 500), rompemos el bucle
                    hasMoreData = false;
                    return new List<Models.User>(); // O maneja el error como prefieras
                }
            }
                
            var filterSet = new HashSet<string>(distinctIds);

            var Users_list = allDistinctIds
                .Where(u => filterSet.Contains(u.email))
                .Distinct().ToList();

            return Users_list;
        }
    }
}