using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Lockbox.Api.MongoDb
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoDatabase _database;

        public UserRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<User> GetAsync(string username)
            => await Users().AsQueryable().FirstOrDefaultAsync(x => x.Username == username.ToLowerInvariant());

        public async Task<int> CountUsersWithRoleAsync(Role role)
            => await Users().AsQueryable().CountAsync(x => x.Role == role);

        public async Task<IEnumerable<string>> GetUsernamesAsync()
            => await Users().AsQueryable().Select(x => x.Username).ToListAsync();

        public async Task AddAsync(User user)
            => await Users().InsertOneAsync(user);

        public async Task DeleteAsync(string username)
            => await Users().DeleteOneAsync(x => x.Username == username.ToLowerInvariant());

        public async Task UpdateAsync(User user)
            => await Users().ReplaceOneAsync(x => x.Username == user.Username.ToLowerInvariant(), user);

        public async Task<User> GetByApiKeyAsync(string apiKey)
            => await Users().AsQueryable().FirstOrDefaultAsync(x => x.ApiKeys.Contains(apiKey));

        public async Task<bool> AnyAsync()
            => await Users().AsQueryable().AnyAsync();

        private IMongoCollection<User> Users()
            => _database.GetCollection<User>("Users");
    }
}