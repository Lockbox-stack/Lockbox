using System.Threading.Tasks;
using Lockbox.Api.Repositories;

namespace Lockbox.Api.Services
{
    public class RecordService : IRecordService
    {
        private readonly IRecordRepository _recordRepository;

        public RecordService(IRecordRepository recordRepository)
        {
            _recordRepository = recordRepository;
        }

        public async Task<object> GetValueAsync(string key)
        {
            var record = await _recordRepository.GetAsync(GetFixedKey(key));

            return record?.Value;
        }

        private string GetFixedKey(string key) => key.ToLowerInvariant();
    }
}