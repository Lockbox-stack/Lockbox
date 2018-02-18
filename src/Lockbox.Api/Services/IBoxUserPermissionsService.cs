using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IBoxUserPermissionsService
    {
        Task<IEnumerable<Permission>> GetAllAsync(string box, string username);
        Task DeleteAllAsync(string box, string username);
        Task UpdateAsync(string box, string username, params Permission[] permissions);
    }
}