using System;

namespace BoltSDK
{
    /// <summary>
    /// Represents a Bolt user with their information
    /// </summary>
    [Serializable]
    public class BoltUser
    {
        public string Email { get; set; }
        public string Locale { get; set; }
        public string Country { get; set; }
        public string DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastActive { get; set; }

        public BoltUser()
        {
            CreatedAt = DateTime.UtcNow;
            LastActive = DateTime.UtcNow;
        }

        public BoltUser(string email, string locale, string country)
        {
            Email = email;
            Locale = locale;
            Country = country;
            CreatedAt = DateTime.UtcNow;
            LastActive = DateTime.UtcNow;
        }
    }
}