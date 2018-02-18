using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Extensions;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly static TimeSpan Expiry = TimeSpan.FromDays(7);
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IEncrypter _encrypter;
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IJwtTokenHandler jwtTokenHandler,
            IEncrypter encrypter,
            IUserRepository userRepository)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _encrypter = encrypter;
            _userRepository = userRepository;
        }

        public async Task<AuthToken> AuthenticateAsync(string username, string password)
        {
            await ValidateCredentialsAsync(username, password);
            var token = _jwtTokenHandler.Create(username, Expiry);

            return AuthToken.Create(token, DateTime.UtcNow.AddTicks(Expiry.Ticks));
        }

        private async Task ValidateCredentialsAsync(string username, string password)
        {
            if (username.Empty())
            {
                throw new ArgumentException("Username can not be empty.", 
                    nameof(username));
            }
            if (password.Empty())
            {
                throw new ArgumentException("Password can not be empty.", 
                    nameof(password));
            }

            var user = await _userRepository.GetAsync(username);
            if (user == null)
            {
                throw new ArgumentNullException($"User: '{username}' has not been found.", 
                    nameof(password));
            }
            if (!user.IsActive)
            {
                throw new AuthenticationException($"User '{username}' is not active.");
            }
            if (!user.ValidatePassword(password, _encrypter))
            {
                throw new AuthenticationException($"Invalid credentials.");
            }
        }
    }
}