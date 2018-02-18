using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IInitializationService
    {
        Task<string> InitializeAsync(string username, string password);
    }
}