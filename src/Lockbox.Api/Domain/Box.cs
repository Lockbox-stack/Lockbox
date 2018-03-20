using System;
using System.Collections.Generic;
using System.Linq;
using Lockbox.Api.Extensions;

namespace Lockbox.Api.Domain
{
    public class Box
    {
        private ISet<BoxUser> _users = new HashSet<BoxUser>();
        private ISet<Entry> _entries = new HashSet<Entry>();
        public string Name { get; protected set; }
        public string Owner { get; protected set; }
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

        public Box(string name, User owner)
        {
            if (name.Empty())
                throw new ArgumentException("Box name can not be empty.", nameof(name));
            if (!name.IsValidName())
                throw new ArgumentException($"Box name '{name}' is invalid.", nameof(name));
            if (name.Length > 100)
                throw new ArgumentException("Box name can not have more than 100 characters.", nameof(name));

            Name = name.Trim().ToLowerInvariant();
            Owner = owner.Username;
            Entries = Enumerable.Empty<Entry>();

            var boxUser = new BoxUser(owner, BoxRole.BoxAdmin);
            boxUser.AddPermission(Permission.CreateEntry);
            boxUser.AddPermission(Permission.DeleteEntry);
            boxUser.AddPermission(Permission.ReadEntry);
            boxUser.AddPermission(Permission.ReadEntryKeys);

            if (owner.IsActive)
                boxUser.Activate();

            AddUser(boxUser);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasManagementAccess(string username)
        {
            if (!HasAccess(username))
                return false;

            var user = GetUser(username);

            return Owner == username || user.Role == BoxRole.BoxAdmin;
        }

        public bool HasAccess(string username)
        {
            var user = GetUser(username);

            return user != null && user.IsActive;
        }

        public void AddEntry(Entry entry)
        {
            DeleteEntry(entry.Key);
            _entries.Add(entry);
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteEntry(string key)
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

        public void DeleteUser(string username)
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