using System;
using MixBrev.Domain;
using MixBrev.Models;
using Newtonsoft.Json.Linq;
using sib_api_v3_sdk.Api;
using sib_api_v3_sdk.Client;
using sib_api_v3_sdk.Model;


namespace MixBrev.Services
{
    public class BrevoService : IBrevoService
    {

        private readonly IConfiguration _configuration;

        public BrevoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SyncUserToBrevoAsync(User user, string cohortId)
        {
            Configuration.Default.ApiKey["api-key"] = _configuration["Brevo:ApiKey"];
            var apiInstance = new ContactsApi();


            long? numericListId = cohortId switch
            {
                "6027991" => 23, // Sign-up - Prompt 
                "6032434" => 25, // Cohort Matching-Job
                "6028090" => 26, // Cohort Prompt-Matching
                "6032435" => 27, // Cohort Job-Pipeline
                "6032438" => 28, // Cohort Pipeline-Hire
                "6032445" => 29, // Cohort First-Hire
                "6039050" => 30, // Test    
                _ => null        
            };

            // Si no hay lista asignada, podrías decidir no seguir o solo actualizar atributos
            if (numericListId == null) return false;

            try
            {
                var attributes = new JObject {
            { "FIRSTNAME", user.first_name },
            { "LASTNAME", user.last_names ?? "" }, // Si last_names es null, se asigna una cadena vacía para evitar errores de Brevo
            { "COHORT", cohortId },
            { "COMPANY", user.company_id },
            {"EXT_ID", user.user_account_id ?? ""}

        };

                var listIds = new List<long?> { numericListId };

                // Crear contacto (si no existe)
                var createContact = new CreateContact(
                    email: user.email,
                    attributes: attributes,
                    listIds: listIds
                );

                await apiInstance.CreateContactAsync(createContact);
                return true;
            }
            catch (ApiException e)
            {

            //    Console.WriteLine("--- ERROR DE BREVO DETECTADO ---");
              //  Console.WriteLine($"Status: {e.ErrorCode}");
                //Console.WriteLine($"Detalle: {e.ErrorContent}");
                //Console.WriteLine("---------------------------------");

                // Si ya existe (400), actualizamos y aseguramos que entre en la lista
                if (e.ErrorCode == 400)
                {
                    var updateContact = new UpdateContact(
                        attributes: new JObject { { "COHORT", cohortId } },
                        listIds: new List<long?> { numericListId }
                    );

                    await apiInstance.UpdateContactAsync(user.email, updateContact);
                    return true;
                }
                return false;
            }
        }
    }
}
