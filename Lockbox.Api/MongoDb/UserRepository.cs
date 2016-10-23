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
            => await Users().AsQueryable().FirstOrDefaultAsync(x => x.Username == username);

        public async Task AddAsync(User user)
            => await Users().InsertOneAsync(user);

        public async Task DeleteAsync(string username)
            => await Users().DeleteOneAsync(x => x.Username == username);

        public async Task UpdateAsync(User user)
            => await Users().ReplaceOneAsync(x => x.Username == user.Username, user);

        public async Task<User> GetByApiKeyAsync(string apiKey)
            => await Users().AsQueryable().FirstOrDefaultAsync(x => x.ApiKeys.Contains(apiKey));

        public async Task<bool> AnyAsync()
            => await Users().AsQueryable().AnyAsync();

        private IMongoCollection<User> Users()
            => _database.GetCollection<User>("Users");
    }
}