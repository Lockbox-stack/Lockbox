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

        public EntryService(IEncrypter encrypter, IEntryRepository entryRepository)
        {
            _encrypter = encrypter;
            _entryRepository = entryRepository;
        }

        public async Task<object> GetValueAsync(string key, string encryptionKey)
        {
            var entry = await _entryRepository.GetAsync(key);
            if (entry == null)
                return null;
            if (entry.Expiry <= DateTime.UtcNow)
                return null;

            var value = _encrypter.Decrypt(entry.Value, entry.Salt, encryptionKey);

            return JsonConvert.DeserializeObject(value);
        }

        public async Task<IEnumerable<string>> GetKeysAsync()
            => await _entryRepository.GetKeysAsync();

        public async Task CreateAsync(string key, object value, string author, string encryptionKey)
        {
            await DeleteAsync(key);
            var randomSecureKey = _encrypter.GetRandomSecureKey();
            var salt = _encrypter.GetSalt(randomSecureKey);
            var serializedValue = JsonConvert.SerializeObject(value);
            var encryptedValue = _encrypter.Encrypt(serializedValue, salt, encryptionKey);
            var entry = new Entry(key, encryptedValue, salt, author);
            await _entryRepository.AddAsync(entry);
        }

        public async Task DeleteAsync(string key)
        {
            var entry = await _entryRepository.GetAsync(key);
            if (entry == null)
                return;

            await _entryRepository.DeleteAsync(key);
        }
    }
}