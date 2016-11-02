using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lockbox.Api.Domain;
using Lockbox.Api.Repositories;
using Newtonsoft.Json;
using NLog;

namespace Lockbox.Api.Services
{
    public class EntryService : IEntryService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IEncrypter _encrypter;
        private readonly IBoxRepository _boxRepository;

        public EntryService(IEncrypter encrypter, IBoxRepository boxRepository)
        {
            _encrypter = encrypter;
            _boxRepository = boxRepository;
        }

        public async Task<object> GetValueAsync(string box, string key, string encryptionKey)
        {
            var entryBox = await _boxRepository.GetAsync(box);
            if (entryBox == null)
                throw new ArgumentException($"Box {box} has not been found.");

            var entry = entryBox.GetEntry(key);
            if (entry == null)
                return null;

            var value = _encrypter.Decrypt(entry.Value, entry.Salt, encryptionKey);

            return JsonConvert.DeserializeObject(value);
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string box)
            => await _boxRepository.GetNamesAsync();

        public async Task CreateAsync(string box, string key, object value, string author, string encryptionKey)
        {
            var entryBox = await _boxRepository.GetAsync(box);
            if (entryBox == null)
                throw new ArgumentException($"Box {box} has not been found.");

            await DeleteAsync(box, key);
            var randomSecureKey = _encrypter.GetRandomSecureKey();
            var salt = _encrypter.GetSalt(randomSecureKey);
            var serializedValue = JsonConvert.SerializeObject(value);
            var encryptedValue = _encrypter.Encrypt(serializedValue, salt, encryptionKey);
            var entry = new Entry(key, encryptedValue, salt, author);
            entryBox.AddEntry(entry);
            await _boxRepository.UpdateAsync(entryBox);
            Logger.Info($"Eentry '{key}' was added to the box '{box}'.");
        }

        public async Task DeleteAsync(string box, string key)
        {
            var entryBox = await _boxRepository.GetAsync(box);
            if (entryBox == null)
                throw new ArgumentException($"Box {box} has not been found.");

            entryBox.DeleteEntry(key);
            await _boxRepository.UpdateAsync(entryBox);
            Logger.Info($"Entry '{key}' was deleted from the box '{box}'.");
        }
    }
}