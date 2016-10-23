using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using Newtonsoft.Json;

namespace Lockbox.Api.Services
{
    public class EntryService : IEntryService
    {
        private readonly IEncrypter _encrypter;
        private readonly IEntryRepository _entryRepository;
        private readonly LockboxSettings _lockboxSettings;

        public EntryService(IEncrypter encrypter, IEntryRepository entryRepository, LockboxSettings lockboxSettings)
        {
            _encrypter = encrypter;
            _entryRepository = entryRepository;
            _lockboxSettings = lockboxSettings;
        }

        public async Task<object> GetValueAsync(string key)
        {
            var entry = await _entryRepository.GetAsync(GetFixedKey(key));
            if (entry == null)
                return null;
            if (entry.Expiry <= DateTime.UtcNow)
                return null;

            var value = _encrypter.Decrypt(entry.Value, entry.Salt, _lockboxSettings.EncryptionKey);

            return JsonConvert.DeserializeObject(value);
        }

        public async Task<IEnumerable<string>> GetKeysAsync()
            => await _entryRepository.GetKeysAsync();

        public async Task CreateAsync(string key, object value)
        {
            await DeleteAsync(key);
            var fixedKey = GetFixedKey(key);
            var randomSecureKey = _encrypter.GetRandomSecureKey();
            var salt = _encrypter.GetSalt(randomSecureKey);
            var serializedValue = JsonConvert.SerializeObject(value);
            var encryptedValue = _encrypter.Encrypt(serializedValue, salt, _lockboxSettings.EncryptionKey);
            var entry = new Entry(fixedKey, encryptedValue, salt);
            await _entryRepository.AddAsync(entry);
        }

        public async Task DeleteAsync(string key)
        {
            var fixedKey = GetFixedKey(key);
            var entry = await _entryRepository.GetAsync(fixedKey);
            if (entry == null)
                return;

            await _entryRepository.DeleteAsync(fixedKey);
        }

        private string GetFixedKey(string key) => key?.ToLowerInvariant();
    }
}