using System.Threading.Tasks;

namespace Lockbox.Client
{
    public interface ILockboxAccountClient
    {
        Task<string> CreateApiKeyAsync();
        Task DeleteApiKeyAsync(string key);
    }
}