using System.Runtime.InteropServices;
using MixBrev.Models;

namespace MixBrev.Domain
{
    public interface IMixpanelService
    {
        // Vamos a tarer una lista de string de IDS
        // Toca revisar si son objetos y no solo string, pero por ahora vamos a asumir que son string
        Task<List<string>> GetMixpanelDataAsync( string cohortID);
        Task<List<User>> GetUserMixpanel(string cohortID);
    }
}
