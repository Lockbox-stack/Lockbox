using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Repositories
{
    public interface IBoxRepository
    {
        Task<Box> GetAsync(string name);
        Task<IEnumerable<string>> GetNamesAsync();
        Task AddAsync(Box box);
        Task DeleteAsync(string name);
        Task UpdateAsync(Box box);
    }
}