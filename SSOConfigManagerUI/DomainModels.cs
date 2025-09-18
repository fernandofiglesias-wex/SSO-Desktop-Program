using System;
using System.Collections.Generic;
using System.Linq;

namespace SSOConfigManagerUI.Models
{
    /// <summary>
    /// Represents an SSO property with its value and mask status
    /// </summary>
    public class SSOProperty
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public bool IsMasked { get; set; }

        public SSOProperty()
        {
        }

        public SSOProperty(string key, object value, bool isMasked = false)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value;
            IsMasked = isMasked;
        }

        public string ValueAsString => Value?.ToString() ?? string.Empty;

        public override string ToString()
        {
            return $"{Key}: {(IsMasked ? "***" : ValueAsString)}";
        }

        public override bool Equals(object obj)
        {
            if (obj is SSOProperty other)
            {
                return string.Equals(Key, other.Key, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key?.ToLowerInvariant().GetHashCode() ?? 0;
        }
    }

    /// <summary>
    /// Represents an SSO Application with its metadata and properties
    /// </summary>
    public class SSOApplication
    {
        private readonly Dictionary<string, SSOProperty> _properties;

        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactInfo { get; set; }
        public string UserAccount { get; set; }
        public string AdminAccount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModified { get; set; }

        public IReadOnlyDictionary<string, SSOProperty> Properties => _properties;

        public SSOApplication()
        {
            _properties = new Dictionary<string, SSOProperty>(StringComparer.OrdinalIgnoreCase);
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
        }

        public SSOApplication(string name, string description = "") : this()
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
        }

        public void AddProperty(SSOProperty property)
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            if (string.IsNullOrWhiteSpace(property.Key)) throw new ArgumentException("Property key cannot be empty");

            _properties[property.Key] = property;
            LastModified = DateTime.Now;
        }

        public void AddProperty(string key, object value, bool isMasked = false)
        {
            AddProperty(new SSOProperty(key, value, isMasked));
        }

        public bool RemoveProperty(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) return false;
            
            bool removed = _properties.Remove(key);
            if (removed)
            {
                LastModified = DateTime.Now;
            }
            return removed;
        }

        public SSOProperty GetProperty(string key)
        {
            return _properties.TryGetValue(key ?? string.Empty, out var property) ? property : null;
        }

        public bool HasProperty(string key)
        {
            return _properties.ContainsKey(key ?? string.Empty);
        }

        public void ClearProperties()
        {
            _properties.Clear();
            LastModified = DateTime.Now;
        }

        public List<SSOProperty> GetPropertiesList()
        {
            return _properties.Values.ToList();
        }

        public Dictionary<string, object> GetPropertiesAsDictionary()
        {
            return _properties.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Value);
        }

        public override string ToString()
        {
            return $"{Name} ({_properties.Count} properties)";
        }
    }

    /// <summary>
    /// Represents application creation settings
    /// </summary>
    public class ApplicationSettings
    {
        public string DefaultContactInfo { get; set; } = "BizTalk@WexInc.com";
        public string DefaultUserAccount { get; set; } = "SSO Affiliate Administrators";
        public string DefaultAdminAccount { get; set; } = "SSO Administrators";
        public int ApplicationFlags { get; set; } = 0;

        public ApplicationSettings()
        {
            // Set default SSO flags using the Microsoft interop constants
            ApplicationFlags = Microsoft.EnterpriseSingleSignOn.Interop.SSOFlag.SSO_FLAG_APP_CONFIG_STORE | 
                             Microsoft.EnterpriseSingleSignOn.Interop.SSOFlag.SSO_FLAG_SSO_WINDOWS_TO_EXTERNAL | 
                             Microsoft.EnterpriseSingleSignOn.Interop.SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;
        }
    }
}