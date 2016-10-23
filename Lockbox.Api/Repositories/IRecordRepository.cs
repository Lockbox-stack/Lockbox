using System.Threading.Tasks;
using Lockbox.Api.Domain;

namespace Lockbox.Api.Repositories
{
    public interface IRecordRepository
    {
        Task<Record> GetAsync(string key);
        Task AddAsync(Record record);
        Task DeleteAsync(string key);
    }
}