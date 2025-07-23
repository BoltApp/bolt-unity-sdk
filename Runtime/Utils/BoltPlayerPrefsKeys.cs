using System;

namespace BoltApp
{
    /// <summary>
    /// Static class containing all PlayerPrefs keys used throughout the Bolt SDK
    /// This centralizes magic strings to prevent typos and make maintenance easier
    /// </summary>
    public static class BoltPlayerPrefsKeys
    {
        /// <summary>
        /// Prefix used for all Bolt SDK PlayerPrefs keys
        /// </summary>
        public const string PREFIX = "BoltSDK_";

        /// <summary>
        /// User-specific prefix for user-related PlayerPrefs keys
        /// </summary>
        public const string USER_PREFIX = "bolt_user_";

        // Device-related keys
        /// <summary>
        /// Key for storing device locale
        /// </summary>
        public const string DEVICE_LOCALE = "deviceLocale";

        /// <summary>
        /// Key for storing user device locale (with user prefix)
        /// </summary>
        public const string USER_DEVICE_LOCALE = USER_PREFIX + "deviceLocale";

        /// <summary>
        /// Key for storing user device country (with user prefix)
        /// </summary>
        public const string USER_DEVICE_COUNTRY = USER_PREFIX + "deviceCountry";
    }
}