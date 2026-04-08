using System;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MixBrev.Domain;
using MixBrev.Models;
using Supabase;

// es static porque necesitamos acceder a las constantes de Postgrest para los filtros, como Operator.In
using static Postgrest.Constants;

namespace MixBrev.Services
{
    public class SupabaseService : ISupabaseService
    {
        private readonly string _url;
        private readonly string _key;
        public SupabaseService(IConfiguration configuration)
        {
            _url = configuration["Supabase:Url"];
            _key = configuration["Supabase:Key"];
        }
        public async Task<List<UserAccount>> GetUsersDetailsAsync(List<string> distinctIds)

        {
            if (distinctIds == null || !distinctIds.Any()) return new List<UserAccount>();

            var options = new SupabaseOptions { AutoConnectRealtime = false };
            var client = new Supabase.Client(_url, _key, options);
            await client.InitializeAsync();

            // Consultamos la tabla "User_accounts" filtrando por los IDs que traemos de Mixpanel
            var result = await client
                .From<UserAccount>()
                .Select("id, email, phone_number,first_name, last_names")
                .Filter("id", Operator.In, distinctIds)
                .Get();

            return result.Models;
        }
    }
}
