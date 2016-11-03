using System;
using System.Collections.Generic;
using System.Linq;

namespace Lockbox.Api.Domain
{
    public class BoxUser
    {
        private ISet<Permission> _permissions = new HashSet<Permission>();

        public string Username { get; protected set; }
        public BoxRole Role { get; protected set; }
        public bool IsActive { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        public IEnumerable<Permission> Permissions
        {
            get { return _permissions; }
            set { _permissions = new HashSet<Permission>(value); }
        }

        protected BoxUser()
        {
        }

        public BoxUser(User user, BoxRole role = BoxRole.BoxUser)
        {
            Username = user.Username.ToLowerInvariant();;
            SetRole(role);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRole(BoxRole role)
        {
            if (Role == role)
                return;

            Role = role;
            UpdatedAt = DateTime.UtcNow;
        }

        public void AddPermission(Permission permission)
        {
            if (Permissions.Contains(permission))
                return;

            _permissions.Add(permission);
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeletePermission(Permission permission)
        {
            if (!Permissions.Contains(permission))
                return;

            _permissions.Add(permission);
            UpdatedAt = DateTime.UtcNow;
        }

        public void DeleteAllPermissions()
        {
            if(!_permissions.Any())
                return;

            _permissions.Clear();
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive)
                return;

            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Lock()
        {
            if (!IsActive)
                return;

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}