using MixBrev.Models;

namespace MixBrev.Domain
{
    public interface IDataMicro
    {

        Task<List<User>> GetUsersDetailsAsync(List<string> distinctIds);
    }
}
