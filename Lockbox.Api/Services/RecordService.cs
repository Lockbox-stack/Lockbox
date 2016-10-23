using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using Newtonsoft.Json;

namespace Lockbox.Api.Services
{
    public class RecordService : IRecordService
    {
        private readonly IEncrypter _encrypter;
        private readonly IRecordRepository _recordRepository;
        private readonly LockboxSettings _lockboxSettings;

        public RecordService(IEncrypter encrypter, IRecordRepository recordRepository, LockboxSettings lockboxSettings)
        {
            _encrypter = encrypter;
            _recordRepository = recordRepository;
            _lockboxSettings = lockboxSettings;
        }

        public async Task<object> GetValueAsync(string key)
        {
            var record = await _recordRepository.GetAsync(GetFixedKey(key));
            if (record == null)
                return null;

            var value = _encrypter.Decrypt(record.Value, record.Salt, _lockboxSettings.EncryptionKey);

            return JsonConvert.DeserializeObject(value);
        }

        public async Task CreateAsync(string key, object value)
        {
            var fixedKey = GetFixedKey(key);
            var randomSecureKey = _encrypter.GetRandomSecureKey();
            var salt = _encrypter.GetSalt(randomSecureKey);
            var serializedValue = JsonConvert.SerializeObject(value);
            var encryptedValue = _encrypter.Encrypt(serializedValue, salt, _lockboxSettings.EncryptionKey);
            var record = new Record(fixedKey, encryptedValue, salt);
            await _recordRepository.AddAsync(record);
        }

        public async Task RemoveAsync(string key)
        {
            throw new System.NotImplementedException();
        }

        private string GetFixedKey(string key) => key?.ToLowerInvariant();
    }
}