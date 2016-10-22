using System.Threading.Tasks;
using Lockbox.Core.Domain;

namespace Lockbox.Core.Repositories
{
    public interface IRecordRepository
    {
        Task<Record> GetAsync(string key);
        Task AddAsync(Record record);
        Task DeleteAsync(string key);
    }
}