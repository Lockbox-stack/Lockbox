using System;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using NLog;
using NLog.Fluent;

namespace Lockbox.Api.Services
{
    public class InitializationService : IInitializationService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IApiKeyService _apiKeyService;
        private readonly IUserRepository _userRepository;
        private readonly IEncrypter _encrypter;

        public InitializationService(IApiKeyService apiKeyService, IUserRepository userRepository, IEncrypter encrypter)
        {
            _apiKeyService = apiKeyService;
            _userRepository = userRepository;
            _encrypter = encrypter;
        }

        public async Task<string> InitializeAsync(string username, string password)
        {
            var initialized = await _userRepository.AnyAsync();
            if (initialized)
                throw new InvalidOperationException("Lockbox has been already initialized.");

            var user = new User(username, Role.Admin);
            user.SetPassword(password, _encrypter);
            user.Activate();
            await _userRepository.AddAsync(user);
            var apiKey = await _apiKeyService.CreateAsync(username);
            Logger.Info($"Lockbox initialization has completed. System admin: {username}.");

            return apiKey;
        }
    }
}