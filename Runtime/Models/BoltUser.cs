#nullable enable
using System;
using System.Collections.Generic;

namespace BoltApp
{
    /// <summary>
    /// Represents a Bolt user with their information
    /// </summary>
    [Serializable]
    public class BoltUser
    {
        public string? Email { get; set; }
        public string? Locale { get; set; }
        public string? Country { get; set; }
        public string? DeviceId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public BoltUser(string email, string locale, string country, string deviceId)
        {
            Email = email;
            Locale = locale;
            Country = country;
            DeviceId = deviceId;
            UpdatedAt = DateTime.UtcNow;
            if (CreatedAt == DateTime.MinValue)
            {
                CreatedAt = DateTime.UtcNow;
            }
        }

        public override string ToString()
        {
            Func<string, string> nullString = (string s) => s == null ? "null" : s;
            return $"BoltUser(Email: {nullString(Email)}," +
                $" DeviceId: {nullString(DeviceId)}," +
                $" UserLocale: {nullString(Locale)}," +
                $" UserCountry: {nullString(Country)}," +
                $" UserCreatedAt: {CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")}," +
                $" UserUpdatedAt: {UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")})";
        }

        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "email", Email ?? "" },
                { "device_id", DeviceId ?? "" },
                { "locale", Locale ?? "" },
                { "country", Country ?? "" },
                { "user_created_at", CreatedAt.ToString("yyyy-MM-dd HH:mm:ss") },
                { "user_updated_at", UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss") }
            };
        }
    }
}