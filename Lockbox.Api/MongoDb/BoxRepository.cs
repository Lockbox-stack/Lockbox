using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Lockbox.Api.MongoDb
{
    public class BoxRepository : IBoxRepository
    {
        private readonly IMongoDatabase _database;

        public BoxRepository(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<Box> GetAsync(string name)
            => await Boxes().AsQueryable().FirstOrDefaultAsync(x => x.Name == name.ToLowerInvariant());

        public async Task<IEnumerable<string>> GetNamesAsync()
            => await Boxes().AsQueryable().Select(x => x.Name).ToListAsync();

        public async Task AddAsync(Box box)
            => await Boxes().InsertOneAsync(box);

        public async Task DeleteAsync(string name)
            => await Boxes().DeleteOneAsync(x => x.Name == name.ToLowerInvariant());

        public async Task UpdateAsync(Box box)
            => await Boxes().ReplaceOneAsync(x => x.Name == box.Name, box);

        private IMongoCollection<Box> Boxes()
            => _database.GetCollection<Box>("Boxes");
    }
}