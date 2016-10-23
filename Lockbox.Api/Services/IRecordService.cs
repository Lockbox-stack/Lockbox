using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IRecordService
    {
        Task<object> GetValueAsync(string key);
    }
}