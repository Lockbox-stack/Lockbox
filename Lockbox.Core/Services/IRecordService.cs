using System.Threading.Tasks;

namespace Lockbox.Core.Services
{
    public interface IRecordService
    {
        Task<object> GetValueAsync(string key);
    }
}