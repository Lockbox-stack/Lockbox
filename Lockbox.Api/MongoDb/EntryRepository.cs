using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Lockbox.Api.MongoDb
{
    public class EntryRepository : IEntryRepository
    {
        private readonly IMongoDatabase _database;

        public EntryRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Entry> GetAsync(string key)
            => await Entries().AsQueryable().FirstOrDefaultAsync(x => x.Key == key);

        public async Task<IEnumerable<string>> GetKeysAsync()
            => await Entries().AsQueryable().Select(x => x.Key).ToListAsync();

        public async Task AddAsync(Entry entry)
            => await Entries().InsertOneAsync(entry);

        public async Task DeleteAsync(string key)
            => await Entries().DeleteOneAsync(x => x.Key == key);

        private IMongoCollection<Entry> Entries()
            => _database.GetCollection<Entry>("Entries");
    }
}