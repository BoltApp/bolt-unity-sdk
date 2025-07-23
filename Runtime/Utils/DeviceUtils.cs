using System;
using System.Globalization;
using UnityEngine;

namespace BoltApp
{
    /// <summary>
    /// Utility class for device information
    /// </summary>
    public static class DeviceUtils
    {
        /// <summary>
        /// Gets the device locale in ISO format (e.g., "en-US")
        /// </summary>
        /// <returns>The device locale</returns>
        public static string GetDeviceLocale()
        {
            try
            {
                var locale = PlayerPrefs.GetString(BoltPlayerPrefsKeys.DEVICE_LOCALE);
                if (!string.IsNullOrEmpty(locale))
                {
                    return locale;
                }

                var culture = CultureInfo.CurrentCulture;
                return $"{culture.TwoLetterISOLanguageName}-{culture.Name.Split('-')[1]}";
            }
            catch
            {
                return GetLocaleFromSystemLanguage(Application.systemLanguage);
            }
        }

        public static void SetDeviceLocale(string locale)
        {
            PlayerPrefs.SetString(BoltPlayerPrefsKeys.USER_DEVICE_LOCALE, locale);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets the device country code
        /// </summary>
        /// <returns>The country code</returns>
        public static string GetDeviceCountry()
        {
            try
            {
                var culture = CultureInfo.CurrentCulture;
                return culture.Name.Split('-')[1];
            }
            catch
            {
                // Fallback to country code if the system language is not supported
                return GetCountryFromSystemLanguage(Application.systemLanguage);
            }
        }

        public static void SetDeviceCountry(string country)
        {
            PlayerPrefs.SetString(BoltPlayerPrefsKeys.USER_DEVICE_COUNTRY, country);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Gets a unique device identifier
        /// </summary>
        /// <returns>The device identifier</returns>
        public static string GetDeviceId()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }

        /// <summary>
        /// Gets the device model
        /// </summary>
        /// <returns>The device model</returns>
        public static string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }

        /// <summary>
        /// Gets the operating system
        /// </summary>
        /// <returns>The operating system</returns>
        public static string GetOperatingSystem()
        {
            return SystemInfo.operatingSystem;
        }

        /// <summary>
        /// Gets the Unity version
        /// </summary>
        /// <returns>The Unity version</returns>
        public static string GetUnityVersion()
        {
            return Application.unityVersion;
        }

        /// <summary>
        /// Gets the application version
        /// </summary>
        /// <returns>The application version</returns>
        public static string GetAppVersion()
        {
            return Application.version;
        }

        /// <summary>
        /// Gets the application bundle identifier
        /// </summary>
        /// <returns>The bundle identifier</returns>
        public static string GetBundleIdentifier()
        {
            return Application.identifier;
        }

        /// <summary>
        /// Checks if the device is connected to the internet
        /// </summary>
        /// <returns>True if connected, false otherwise</returns>
        public static bool IsConnectedToInternet()
        {
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        /// <summary>
        /// Gets the network reachability status
        /// </summary>
        /// <returns>The network reachability</returns>
        public static NetworkReachability GetNetworkReachability()
        {
            return Application.internetReachability;
        }

        /// <summary>
        /// Gets the device language
        /// </summary>
        /// <returns>The device language</returns>
        public static string GetDeviceLanguage()
        {
            return Application.systemLanguage.ToString();
        }

        /// <summary>
        /// Gets the target platform
        /// </summary>
        /// <returns>The target platform</returns>
        public static RuntimePlatform GetTargetPlatform()
        {
            return Application.platform;
        }

        /// <summary>
        /// Checks if the application is running in the editor
        /// </summary>
        /// <returns>True if running in editor, false otherwise</returns>
        public static bool IsEditor()
        {
            return Application.isEditor;
        }

        /// <summary>
        /// Gets device information as a dictionary
        /// </summary>
        /// <returns>Dictionary containing device information</returns>
        public static System.Collections.Generic.Dictionary<string, string> GetDeviceInfo()
        {
            return new System.Collections.Generic.Dictionary<string, string>
            {
                { "deviceId", GetDeviceId() },
                { "deviceModel", GetDeviceModel() },
                { "operatingSystem", GetOperatingSystem() },
                { "locale", GetDeviceLocale() },
                { "country", GetDeviceCountry() },
                { "language", GetDeviceLanguage() },
                { "platform", GetTargetPlatform().ToString() },
                { "unityVersion", GetUnityVersion() },
                { "appVersion", GetAppVersion() },
                { "bundleIdentifier", GetBundleIdentifier() },
                { "isEditor", IsEditor().ToString() }
            };
        }

        /// <summary>
        /// Fallback mechanism – interprets the system language to a locale string
        /// </summary>
        /// <param name="systemLanguage">The system language</param>
        /// <returns>The locale string</returns>
        private static string GetLocaleFromSystemLanguage(SystemLanguage systemLanguage)
        {
            return systemLanguage switch
            {
                SystemLanguage.English => "en-US",
                SystemLanguage.Spanish => "es-ES",
                SystemLanguage.French => "fr-FR",
                SystemLanguage.German => "de-DE",
                SystemLanguage.Italian => "it-IT",
                SystemLanguage.Portuguese => "pt-PT",
                SystemLanguage.Russian => "ru-RU",
                SystemLanguage.Japanese => "ja-JP",
                SystemLanguage.Korean => "ko-KR",
                SystemLanguage.Chinese or SystemLanguage.ChineseSimplified => "zh-CN",
                SystemLanguage.ChineseTraditional => "zh-TW",
                SystemLanguage.Arabic => "ar-SA",
                SystemLanguage.Dutch => "nl-NL",
                SystemLanguage.Swedish => "sv-SE",
                SystemLanguage.Norwegian => "no-NO",
                SystemLanguage.Danish => "da-DK",
                SystemLanguage.Finnish => "fi-FI",
                SystemLanguage.Polish => "pl-PL",
                SystemLanguage.Czech => "cs-CZ",
                SystemLanguage.Hungarian => "hu-HU",
                SystemLanguage.Romanian => "ro-RO",
                SystemLanguage.Bulgarian => "bg-BG",
                SystemLanguage.Greek => "el-GR",
                SystemLanguage.Turkish => "tr-TR",
                SystemLanguage.Thai => "th-TH",
                SystemLanguage.Vietnamese => "vi-VN",
                SystemLanguage.Indonesian => "id-ID",
                SystemLanguage.Hebrew => "he-IL",
                SystemLanguage.Hindi => "hi-IN",
                SystemLanguage.Afrikaans => "af-ZA",
                SystemLanguage.Basque => "eu-ES",
                SystemLanguage.Catalan => "ca-ES",
                SystemLanguage.Estonian => "et-EE",
                SystemLanguage.Icelandic => "is-IS",
                SystemLanguage.Latvian => "lv-LV",
                SystemLanguage.Lithuanian => "lt-LT",
                SystemLanguage.Slovak => "sk-SK",
                SystemLanguage.Slovenian => "sl-SI",
                SystemLanguage.Ukrainian => "uk-UA",
                _ => "en-US" // Default fallback
            };
        }

        /// <summary>
        /// Fallback mechanism – interprets the system language to a country code
        /// </summary>
        /// <param name="systemLanguage">The system language</param>
        /// <returns>The country code</returns>
        private static string GetCountryFromSystemLanguage(SystemLanguage systemLanguage)
        {
            return systemLanguage switch
            {
                SystemLanguage.English => "US",
                SystemLanguage.Spanish => "ES",
                SystemLanguage.French => "FR",
                SystemLanguage.German => "DE",
                SystemLanguage.Italian => "IT",
                SystemLanguage.Portuguese => "PT",
                SystemLanguage.Russian => "RU",
                SystemLanguage.Japanese => "JP",
                SystemLanguage.Korean => "KR",
                SystemLanguage.Chinese or SystemLanguage.ChineseSimplified => "CN",
                SystemLanguage.ChineseTraditional => "TW",
                SystemLanguage.Arabic => "SA",
                SystemLanguage.Dutch => "NL",
                SystemLanguage.Swedish => "SE",
                SystemLanguage.Norwegian => "NO",
                SystemLanguage.Danish => "DK",
                SystemLanguage.Finnish => "FI",
                SystemLanguage.Polish => "PL",
                SystemLanguage.Czech => "CZ",
                SystemLanguage.Hungarian => "HU",
                SystemLanguage.Romanian => "RO",
                SystemLanguage.Bulgarian => "BG",
                SystemLanguage.Greek => "GR",
                SystemLanguage.Turkish => "TR",
                SystemLanguage.Thai => "TH",
                SystemLanguage.Vietnamese => "VN",
                SystemLanguage.Indonesian => "ID",
                SystemLanguage.Hebrew => "IL",
                SystemLanguage.Hindi => "IN",
                SystemLanguage.Afrikaans => "ZA",
                SystemLanguage.Basque => "ES",
                SystemLanguage.Catalan => "ES",
                SystemLanguage.Estonian => "EE",
                SystemLanguage.Icelandic => "IS",
                SystemLanguage.Latvian => "LV",
                SystemLanguage.Lithuanian => "LT",
                SystemLanguage.Slovak => "SK",
                SystemLanguage.Slovenian => "SI",
                SystemLanguage.Ukrainian => "UA",
                _ => "US" // Default fallback
            };
        }
    }
}