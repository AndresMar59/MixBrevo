using MixBrev.Models;

namespace MixBrev.Domain
{
    public interface IBrevoService
    {

         // Ponemos un bool para verificar si ese usarui ya esta en el segmento. Por que el MixpanelService solo saca la lista
        Task<bool> SyncUserToBrevoAsync(User user, string CohortId);


    }
}
