using MixBrev.Models;

namespace MixBrev.Domain
{
    public interface ISupabaseService
    {

        // Recibe IDs de Mixpanel y devuelve los modelos de usuario con Email y CompanyId
        Task<List<UserAccount>> GetUsersDetailsAsync(List<string> distinctIds);

    }
}
