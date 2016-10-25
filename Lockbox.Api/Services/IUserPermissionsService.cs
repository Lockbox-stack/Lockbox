using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Services
{
    public interface IUserPermissionsService
    {
        Task<IEnumerable<Permission>> GetAllAsync(string username);
        Task DeleteAllAsync(string username);
        Task UpdateAsync(string username, params Permission[] permissions);
    }
}