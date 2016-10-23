using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Lockbox.Api.MongoDb
{
    public class MongoDbRecordRepository : IRecordRepository
    {
        private readonly IMongoDatabase _database;

        public MongoDbRecordRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Record> GetAsync(string key)
            => await Records().AsQueryable().FirstOrDefaultAsync(x => x.Key == key);

        public async Task AddAsync(Record record)
            => await Records().InsertOneAsync(record);

        public async Task DeleteAsync(string key)
            => await Records().DeleteOneAsync(x => x.Key == key);

        private IMongoCollection<Record> Records()
            => _database.GetCollection<Record>("Records");
    }
}