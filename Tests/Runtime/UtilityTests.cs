using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class UtilityTests
    {
        [Test]
        public void JsonUtils_ToJson_ShouldSerializeObject()
        {
            // Arrange
            var testObject = new BoltUser
            {
                Email = "test@example.com",
                Locale = "en-US",
                Country = "US"
            };

            // Act
            var json = JsonUtils.ToJson(testObject);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("test@example.com"));
            Assert.IsTrue(json.Contains("en-US"));
            Assert.IsTrue(json.Contains("US"));
        }

        [Test]
        public void JsonUtils_FromJson_ShouldDeserializeObject()
        {
            // Arrange
            var originalObject = new BoltUser
            {
                Email = "test@example.com",
                Locale = "en-US",
                Country = "US"
            };
            var json = JsonUtils.ToJson(originalObject);

            // Act
            var deserializedObject = JsonUtils.FromJson<BoltUser>(json);

            // Assert
            Assert.IsNotNull(deserializedObject);
            Assert.AreEqual(originalObject.Email, deserializedObject.Email);
            Assert.AreEqual(originalObject.Locale, deserializedObject.Locale);
            Assert.AreEqual(originalObject.Country, deserializedObject.Country);
        }

        [Test]
        public void JsonUtils_FromJson_WithNullJson_ShouldReturnDefault()
        {
            // Act
            var result = JsonUtils.FromJson<BoltUser>(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void JsonUtils_FromJson_WithEmptyJson_ShouldReturnDefault()
        {
            // Act
            var result = JsonUtils.FromJson<BoltUser>("");

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void JsonUtils_TryFromJson_WithValidJson_ShouldReturnTrue()
        {
            // Arrange
            var originalObject = new BoltUser { Email = "test@example.com" };
            var json = JsonUtils.ToJson(originalObject);

            // Act
            var success = JsonUtils.TryFromJson(json, out BoltUser result);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(result);
            Assert.AreEqual(originalObject.Email, result.Email);
        }

        [Test]
        public void JsonUtils_TryFromJson_WithInvalidJson_ShouldReturnFalse()
        {
            // Arrange
            var invalidJson = "{ invalid json }";

            // Act
            var success = JsonUtils.TryFromJson(invalidJson, out BoltUser result);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [Test]
        public void JsonUtils_DictionaryToJson_ShouldSerializeDictionary()
        {
            // Arrange
            var dictionary = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 42 },
                { "key3", true }
            };

            // Act
            var json = JsonUtils.DictionaryToJson(dictionary);

            // Assert
            Assert.IsNotNull(json);
            Assert.IsTrue(json.Contains("key1"));
            Assert.IsTrue(json.Contains("value1"));
        }

        [Test]
        public void JsonUtils_JsonToDictionary_ShouldDeserializeDictionary()
        {
            // Arrange
            var originalDictionary = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 42 }
            };
            var json = JsonUtils.DictionaryToJson(originalDictionary);

            // Act
            var deserializedDictionary = JsonUtils.JsonToDictionary(json);

            // Assert
            Assert.IsNotNull(deserializedDictionary);
            Assert.AreEqual(originalDictionary.Count, deserializedDictionary.Count);
            Assert.AreEqual(originalDictionary["key1"], deserializedDictionary["key1"]);
            Assert.AreEqual(originalDictionary["key2"], deserializedDictionary["key2"]);
        }

        [Test]
        public void JsonUtils_PrettyPrint_ShouldFormatJson()
        {
            // Arrange
            var testObject = new BoltUser { Email = "test@example.com" };
            var json = JsonUtils.ToJson(testObject);

            // Act
            var prettyJson = JsonUtils.PrettyPrint(json);

            // Assert
            Assert.IsNotNull(prettyJson);
            Assert.IsTrue(prettyJson.Contains("\n"));
        }

        [Test]
        public void UrlUtils_AppendQueryParameters_ShouldAppendParameters()
        {
            // Arrange
            var baseUrl = "https://test.com/checkout";
            var parameters = new Dictionary<string, string>
            {
                { "user_id", "123" },
                { "currency", "USD" }
            };

            // Act
            var result = UrlUtils.AppendQueryParameters(baseUrl, parameters);

            // Assert
            Assert.IsTrue(result.Contains("user_id=123"));
            Assert.IsTrue(result.Contains("currency=USD"));
            Assert.IsTrue(result.StartsWith(baseUrl));
        }

        [Test]
        public void UrlUtils_AppendQueryParameters_WithNullParameters_ShouldReturnOriginalUrl()
        {
            // Arrange
            var baseUrl = "https://test.com/checkout";

            // Act
            var result = UrlUtils.AppendQueryParameters(baseUrl, null);

            // Assert
            Assert.AreEqual(baseUrl, result);
        }

        [Test]
        public void UrlUtils_AppendQueryParameters_WithEmptyParameters_ShouldReturnOriginalUrl()
        {
            // Arrange
            var baseUrl = "https://test.com/checkout";
            var parameters = new Dictionary<string, string>();

            // Act
            var result = UrlUtils.AppendQueryParameters(baseUrl, parameters);

            // Assert
            Assert.AreEqual(baseUrl, result);
        }

        [Test]
        public void UrlUtils_ExtractQueryParameters_ShouldExtractParameters()
        {
            // Arrange
            var url = "https://test.com/checkout?user_id=123&currency=USD&status=completed";

            // Act
            var parameters = UrlUtils.ExtractQueryParameters(url);

            // Assert
            Assert.AreEqual(3, parameters.Count);
            Assert.AreEqual("123", parameters["user_id"]);
            Assert.AreEqual("USD", parameters["currency"]);
            Assert.AreEqual("completed", parameters["status"]);
        }

        [Test]
        public void UrlUtils_ExtractQueryParameters_WithNoQuery_ShouldReturnEmptyDictionary()
        {
            // Arrange
            var url = "https://test.com/checkout";

            // Act
            var parameters = UrlUtils.ExtractQueryParameters(url);

            // Assert
            Assert.AreEqual(0, parameters.Count);
        }

        [Test]
        public void UrlUtils_IsValidUrl_WithValidUrl_ShouldReturnTrue()
        {
            // Arrange
            var validUrls = new[]
            {
                "https://test.com",
                "http://test.com",
                "https://test.com/path",
                "https://test.com/path?param=value"
            };

            // Act & Assert
            foreach (var url in validUrls)
            {
                Assert.IsTrue(UrlUtils.IsValidUrl(url), $"URL should be valid: {url}");
            }
        }

        [Test]
        public void UrlUtils_IsValidUrl_WithInvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            var invalidUrls = new[]
            {
                "invalid-url",
                "ftp://test.com",
                "not-a-url",
                ""
            };

            // Act & Assert
            foreach (var url in invalidUrls)
            {
                Assert.IsFalse(UrlUtils.IsValidUrl(url), $"URL should be invalid: {url}");
            }
        }

        [Test]
        public void UrlUtils_GetBaseUrl_ShouldReturnBaseUrl()
        {
            // Arrange
            var fullUrl = "https://test.com/path?param=value";

            // Act
            var baseUrl = UrlUtils.GetBaseUrl(fullUrl);

            // Assert
            Assert.AreEqual("https://test.com/path", baseUrl);
        }

        [Test]
        public void UrlUtils_UrlEncode_ShouldEncodeString()
        {
            // Arrange
            var originalString = "test string with spaces";

            // Act
            var encoded = UrlUtils.UrlEncode(originalString);

            // Assert
            Assert.IsNotNull(encoded);
            Assert.AreNotEqual(originalString, encoded);
        }

        [Test]
        public void UrlUtils_UrlDecode_ShouldDecodeString()
        {
            // Arrange
            var originalString = "test string with spaces";
            var encoded = UrlUtils.UrlEncode(originalString);

            // Act
            var decoded = UrlUtils.UrlDecode(encoded);

            // Assert
            Assert.AreEqual(originalString, decoded);
        }

        [Test]
        public void UrlUtils_CombineUrl_ShouldCombineUrls()
        {
            // Arrange
            var baseUrl = "https://test.com";
            var path = "api/checkout";

            // Act
            var combined = UrlUtils.CombineUrl(baseUrl, path);

            // Assert
            Assert.AreEqual("https://test.com/api/checkout", combined);
        }

        [Test]
        public void UrlUtils_CombineUrl_WithTrailingSlash_ShouldHandleCorrectly()
        {
            // Arrange
            var baseUrl = "https://test.com/";
            var path = "/api/checkout";

            // Act
            var combined = UrlUtils.CombineUrl(baseUrl, path);

            // Assert
            Assert.AreEqual("https://test.com/api/checkout", combined);
        }

        [Test]
        public void UrlUtils_IsDeepLinkCallback_WithValidScheme_ShouldReturnTrue()
        {
            // Arrange
            var url = "myapp://bolt/callback";
            var scheme = "myapp";

            // Act
            var isCallback = UrlUtils.IsDeepLinkCallback(url, scheme);

            // Assert
            Assert.IsTrue(isCallback);
        }

        [Test]
        public void UrlUtils_IsDeepLinkCallback_WithInvalidScheme_ShouldReturnFalse()
        {
            // Arrange
            var url = "myapp://bolt/callback";
            var scheme = "otherapp";

            // Act
            var isCallback = UrlUtils.IsDeepLinkCallback(url, scheme);

            // Assert
            Assert.IsFalse(isCallback);
        }

        [Test]
        public void DeviceUtils_GetDeviceLocale_ShouldReturnValidLocale()
        {
            // Act
            var locale = DeviceUtils.GetDeviceLocale();

            // Assert
            Assert.IsNotNull(locale);
            Assert.IsTrue(locale.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetDeviceCountry_ShouldReturnValidCountry()
        {
            // Act
            var country = DeviceUtils.GetDeviceCountry();

            // Assert
            Assert.IsNotNull(country);
            Assert.IsTrue(country.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetDeviceId_ShouldReturnValidId()
        {
            // Act
            var deviceId = DeviceUtils.GetDeviceId();

            // Assert
            Assert.IsNotNull(deviceId);
            Assert.IsTrue(deviceId.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetDeviceModel_ShouldReturnValidModel()
        {
            // Act
            var model = DeviceUtils.GetDeviceModel();

            // Assert
            Assert.IsNotNull(model);
        }

        [Test]
        public void DeviceUtils_GetOperatingSystem_ShouldReturnValidOS()
        {
            // Act
            var os = DeviceUtils.GetOperatingSystem();

            // Assert
            Assert.IsNotNull(os);
            Assert.IsTrue(os.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetUnityVersion_ShouldReturnValidVersion()
        {
            // Act
            var version = DeviceUtils.GetUnityVersion();

            // Assert
            Assert.IsNotNull(version);
            Assert.IsTrue(version.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetAppVersion_ShouldReturnValidVersion()
        {
            // Act
            var version = DeviceUtils.GetAppVersion();

            // Assert
            Assert.IsNotNull(version);
        }

        [Test]
        public void DeviceUtils_GetBundleIdentifier_ShouldReturnValidIdentifier()
        {
            // Act
            var identifier = DeviceUtils.GetBundleIdentifier();

            // Assert
            Assert.IsNotNull(identifier);
        }

        [Test]
        public void DeviceUtils_IsConnectedToInternet_ShouldReturnBoolean()
        {
            // Act
            var isConnected = DeviceUtils.IsConnectedToInternet();

            // Assert
            // This could be true or false depending on the test environment
            Assert.IsInstanceOf<bool>(isConnected);
        }

        [Test]
        public void DeviceUtils_GetNetworkReachability_ShouldReturnValidValue()
        {
            // Act
            var reachability = DeviceUtils.GetNetworkReachability();

            // Assert
            Assert.IsInstanceOf<NetworkReachability>(reachability);
        }

        [Test]
        public void DeviceUtils_GetDeviceLanguage_ShouldReturnValidLanguage()
        {
            // Act
            var language = DeviceUtils.GetDeviceLanguage();

            // Assert
            Assert.IsNotNull(language);
            Assert.IsTrue(language.Length > 0);
        }

        [Test]
        public void DeviceUtils_GetTargetPlatform_ShouldReturnValidPlatform()
        {
            // Act
            var platform = DeviceUtils.GetTargetPlatform();

            // Assert
            Assert.IsInstanceOf<RuntimePlatform>(platform);
        }

        [Test]
        public void DeviceUtils_IsEditor_ShouldReturnBoolean()
        {
            // Act
            var isEditor = DeviceUtils.IsEditor();

            // Assert
            Assert.IsInstanceOf<bool>(isEditor);
        }

        [Test]
        public void DeviceUtils_GetDeviceInfo_ShouldReturnValidDictionary()
        {
            // Act
            var deviceInfo = DeviceUtils.GetDeviceInfo();

            // Assert
            Assert.IsNotNull(deviceInfo);
            Assert.IsTrue(deviceInfo.Count > 0);
            Assert.IsTrue(deviceInfo.ContainsKey("deviceId"));
            Assert.IsTrue(deviceInfo.ContainsKey("locale"));
            Assert.IsTrue(deviceInfo.ContainsKey("country"));
        }
    }
}