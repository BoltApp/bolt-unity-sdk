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
        public const string PREFIX = "boltsdk_";

        /// <summary>
        /// Key for storing user data
        /// </summary>
        public const string USER_DATA = PREFIX + "user_data";

        /// <summary>
        /// Key for storing user device locale
        /// </summary>
        public const string DEVICE_LOCALE = PREFIX + "device_locale";

        /// <summary>
        /// Key for storing user device country
        /// </summary>
        public const string DEVICE_COUNTRY = PREFIX + "device_country";
    }
}