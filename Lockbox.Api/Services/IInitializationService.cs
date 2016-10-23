using System.Threading.Tasks;

namespace Lockbox.Api.Services
{
    public interface IInitializationService
    {
        Task InitializeAsync(string username, string password);
    }
}