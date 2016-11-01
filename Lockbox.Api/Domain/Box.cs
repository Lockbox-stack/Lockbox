using System;
using System.Collections.Generic;
using System.Linq;

namespace Lockbox.Api.Domain
{
    public class Box
    {
        private ISet<BoxUser> _users = new HashSet<BoxUser>();
        private ISet<Entry> _entries = new HashSet<Entry>();

        public string Name { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        public IEnumerable<Entry> Entries
        {
            get { return _entries; }
            set { _entries = new HashSet<Entry>(value); }
        }

        public IEnumerable<BoxUser> Users
        {
            get { return _users; }
            set { _users = new HashSet<BoxUser>(value); }
        }

        protected Box()
        {
        }

        public Box(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Box name can not be empty.", nameof(name));
            if (name.Length > 100)
                throw new ArgumentException("Box name can not have more than 100 characters.", nameof(name));

            Name = name.ToLowerInvariant();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddEntry(Entry entry)
        {
            if (_entries.Contains(entry))
                return;

            _entries.Add(entry);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveEntry(string key)
        {
            var entry = GetEntry(key);
            if (entry == null)
                return;

            _entries.Remove(entry);
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddUser(BoxUser user)
        {
            if (_users.Contains(user))
                return;

            _users.Add(user);
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveUser(string username)
        {
            var user = GetUser(username);
            if (user == null)
                return;

            _users.Remove(user);
            UpdatedAt = DateTime.UtcNow;
        }

        public Entry GetEntry(string key)
            => _entries.FirstOrDefault(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase));

        public BoxUser GetUser(string username)
            => _users.FirstOrDefault(x => x.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));
    }
}