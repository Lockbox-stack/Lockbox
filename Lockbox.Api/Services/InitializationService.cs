using System;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class InitializationService : IInitializationService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEncrypter _encrypter;

        public InitializationService(IUserRepository userRepository, IEncrypter encrypter)
        {
            _userRepository = userRepository;
            _encrypter = encrypter;
        }

        public async Task InitializeAsync(string username, string password)
        {
            var initialized = await _userRepository.AnyAsync();
            if (initialized)
                throw new InvalidOperationException("Lockbox has been already initialized.");

            var user = new User(username, Role.Admin);
            user.SetPassword(password, _encrypter);
            user.Activate();
            await _userRepository.AddAsync(user);
        }
    }
}