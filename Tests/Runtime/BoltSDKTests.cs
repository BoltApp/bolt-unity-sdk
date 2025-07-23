using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKTests
    {
        private BoltSDK _boltSDK;
        private MockWebLinkService _mockWebLinkService;
        private MockStorageService _mockStorageService;

        [SetUp]
        public void SetUp()
        {
            _mockWebLinkService = new MockWebLinkService();
            _mockStorageService = new MockStorageService();
            _boltSDK = new BoltSDK(_mockWebLinkService, _mockStorageService);
        }

        [TearDown]
        public void TearDown()
        {
            _boltSDK?.Dispose();
        }

        [Test]
        public void Init_WithValidParameters_ShouldInitializeSuccessfully()
        {
            // Arrange
            var gameId = "test-game-id";
            var deepLinkAppName = "test-deep-link-app";

            // Act
            _boltSDK.Init(gameId, deepLinkAppName);

            // Assert
            Assert.IsTrue(_boltSDK.IsInitialized);
            Assert.AreEqual(gameId, _mockStorageService.GetString("gameId"));
            Assert.AreEqual(deepLinkAppName, _mockStorageService.GetString("deepLinkAppName"));
        }

        [Test]
        public void Init_WithoutParameters_ShouldThrowExceptionWhenNoConfigExists()
        {
            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.Init());
            Assert.AreEqual("No Bolt SDK Configuration found. Please create one using Tools > Bolt SDK > Configuration", exception.Message);
        }

        [Test]
        public void Init_WithValidGameIdOnly_ShouldInitializeSuccessfully()
        {
            // Arrange
            var gameId = "test-game-id";

            // Act
            _boltSDK.Init(gameId);

            // Assert
            Assert.IsTrue(_boltSDK.IsInitialized);
            Assert.AreEqual(gameId, _mockStorageService.GetString("gameId"));
        }

        [Test]
        public void Init_WithNullGameId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.Init(null));
            Assert.AreEqual("Game ID cannot be null or empty", exception.Message);
        }

        [Test]
        public void Init_WithEmptyGameId_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.Init(""));
            Assert.AreEqual("Game ID cannot be null or empty", exception.Message);
        }

        [Test]
        public void OpenCheckout_WhenNotInitialized_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<BoltSDKNotInitializedException>(() => _boltSDK.OpenCheckout("test-product"));
            Assert.AreEqual("Bolt SDK is not initialized. Call Init() first.", exception.Message);
        }

        [Test]
        public void OpenCheckout_WithNullProductId_ShouldThrowException()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.OpenCheckout(null));
            Assert.AreEqual("Checkout link cannot be null or empty", exception.Message);
        }

        [Test]
        public void OpenCheckout_WithEmptyProductId_ShouldThrowException()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.OpenCheckout(""));
            Assert.AreEqual("Checkout link cannot be null or empty", exception.Message);
        }

        [Test]
        public void OpenCheckout_WithValidCheckoutLink_ShouldOpenWebLink()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            var checkoutOpened = false;
            _boltSDK.onCheckoutOpen += () => checkoutOpened = true;

            // Act
            _boltSDK.OpenCheckout("https://test.com/checkout");

            // Assert
            Assert.IsTrue(_mockWebLinkService.IsWebLinkOpen);
            Assert.AreEqual("https://test.com/checkout", _mockWebLinkService.CurrentUrl);
            Assert.IsTrue(checkoutOpened);
        }

        [Test]
        public void OpenCheckout_WithExtraParams_ShouldAppendToUrl()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            var extraParams = new Dictionary<string, string>
            {
                { "user_level", "10" },
                { "currency", "EUR" }
            };

            // Act
            _boltSDK.OpenCheckout("https://test.com/checkout", extraParams);

            // Assert
            Assert.IsTrue(_mockWebLinkService.CurrentUrl.Contains("user_level=10"));
            Assert.IsTrue(_mockWebLinkService.CurrentUrl.Contains("currency=EUR"));
        }

        [Test]
        public void HandleWeblinkCallback_WithValidCallback_ShouldParseTransaction()
        {
            // Arrange
            _boltSDK.Init("test-game-id");
            var callbackUrl = "https://bolt.com/callback?transaction_id=123&status=completed&amount=9.99&currency=USD";

            TransactionResult result = null;
            _boltSDK.onTransactionComplete += (transactionResult) => result = transactionResult;

            // Act
            var transactionResult = _boltSDK.HandleWeblinkCallback(callbackUrl);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("123", result.TransactionId);
            Assert.AreEqual(TransactionStatus.Completed, result.Status);
            Assert.AreEqual(9.99m, result.Amount);
            Assert.AreEqual("USD", result.Currency);
        }

        [Test]
        public void HandleWeblinkCallback_WithFailedTransaction_ShouldParseCorrectly()
        {
            // Arrange
            _boltSDK.Init("test-api-key", "test-game-id");
            var callbackUrl = "https://bolt.com/callback?transaction_id=456&status=failed&amount=0&currency=USD";

            TransactionResult result = null;

            // Act
            _boltSDK.HandleWeblinkCallback(callbackUrl, (transactionResult) => result = transactionResult);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("456", result.TransactionId);
            Assert.AreEqual(TransactionStatus.Failed, result.Status);
            Assert.IsFalse(result.IsSuccessful);
        }

        [Test]
        public void GetUnacknowledgedTransactions_ShouldReturnPendingTransactions()
        {
            // Arrange
            _boltSDK.Init("test-game-id");
            _mockStorageService.SetString("pendingTransactions", JsonUtils.ToJson(new[] { "tx1", "tx2", "tx3" }));

            // Act
            var unacknowledged = _boltSDK.GetUnacknowledgedTransactions();

            // Assert
            Assert.AreEqual(3, unacknowledged.Length);
            Assert.Contains("tx1", unacknowledged);
            Assert.Contains("tx2", unacknowledged);
            Assert.Contains("tx3", unacknowledged);
        }

        [Test]
        public void AcknowledgeTransactions_WithValidIds_ShouldReturnTrue()
        {
            // Arrange
            _boltSDK.Init("test-game-id");
            _mockStorageService.SetString("pendingTransactions", JsonUtils.ToJson(new[] { "tx1", "tx2" }));

            // Act
            var result = _boltSDK.AcknowledgeTransactions(new[] { "tx1", "tx2" });

            // Assert
            Assert.IsTrue(result);
            var pending = _boltSDK.GetUnacknowledgedTransactions();
            Assert.AreEqual(0, pending.Length);
        }

        [Test]
        public void AcknowledgeTransactions_WithInvalidIds_ShouldReturnFalse()
        {
            // Arrange
            _boltSDK.Init("test-game-id");
            _mockStorageService.SetString("pendingTransactions", JsonUtils.ToJson(new[] { "tx1" }));

            // Act
            var result = _boltSDK.AcknowledgeTransactions(new[] { "tx1", "tx2" });

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void DeviceLocale_ShouldReturnValidLocale()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            // Act
            var locale = _boltSDK.DeviceLocale;

            // Assert
            Assert.IsNotNull(locale);
            Assert.IsTrue(locale.Length > 0);
        }

        [Test]
        public void BoltUser_ShouldBeInitializedAfterInit()
        {
            // Arrange & Act
            _boltSDK.Init("test-game-id");

            // Assert
            Assert.IsNotNull(_boltSDK.BoltUser);
            Assert.AreEqual(DeviceUtils.GetDeviceLocale(), _boltSDK.BoltUser.Locale);
            Assert.AreEqual(DeviceUtils.GetDeviceCountry(), _boltSDK.BoltUser.Country);
        }

        [Test]
        public void Dispose_ShouldCleanupResources()
        {
            // Arrange
            _boltSDK.Init("test-game-id");

            // Act
            _boltSDK.Dispose();

            // Assert
            Assert.IsFalse(_boltSDK.IsInitialized);
        }

        [Test]
        public void Init_WithBoltConfig_ShouldInitializeSuccessfully()
        {
            // Arrange
            var config = ScriptableObject.CreateInstance<BoltConfig>();
            config.gameId = "test-game-id";
            config.deepLinkAppName = "test-deep-link-app";
            config.environment = BoltConfig.Environment.Development;

            // Act
            _boltSDK.Init(config);

            // Assert
            Assert.IsTrue(_boltSDK.IsInitialized);
            Assert.AreEqual("test-game-id", _mockStorageService.GetString("gameId"));
            Assert.AreEqual("test-deep-link-app", _mockStorageService.GetString("deepLinkAppName"));
        }

        [Test]
        public void Init_WithNullBoltConfig_ShouldThrowException()
        {
            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.Init((BoltConfig)null));
            Assert.AreEqual("Configuration cannot be null", exception.Message);
        }

        [Test]
        public void Init_WithInvalidBoltConfig_ShouldThrowException()
        {
            // Arrange
            var config = ScriptableObject.CreateInstance<BoltConfig>();
            config.gameId = ""; // Invalid - empty game ID
            config.deepLinkAppName = "test-deep-link-app";

            // Act & Assert
            var exception = Assert.Throws<BoltSDKException>(() => _boltSDK.Init(config));
            Assert.AreEqual("Configuration is not valid. Please check the error messages above.", exception.Message);
        }
    }

    /// <summary>
    /// Mock web link service for testing
    /// </summary>
    public class MockWebLinkService : IWebLinkService
    {
        public event Action OnWebLinkOpened;
        public event Action OnWebLinkClosed;
        public event Action<string> OnUrlLoaded;
        public event Action<string> OnError;

        public bool IsWebLinkOpen { get; private set; }
        public string CurrentUrl { get; private set; }

        public void OpenWebLink(string url, Dictionary<string, string> extraParams = null)
        {
            CurrentUrl = url;
            if (extraParams != null && extraParams.Count > 0)
            {
                CurrentUrl = UrlUtils.AppendQueryParameters(url, extraParams);
            }

            IsWebLinkOpen = true;
            OnWebLinkOpened?.Invoke();
            OnUrlLoaded?.Invoke(CurrentUrl);
        }

        public void CloseWebLink()
        {
            IsWebLinkOpen = false;
            CurrentUrl = "";
            OnWebLinkClosed?.Invoke();
        }

        public void SetTitle(string title) { }
        public void SetCanClose(bool canClose) { }
    }

    /// <summary>
    /// Mock storage service for testing
    /// </summary>
    public class MockStorageService : IStorageService
    {
        private readonly Dictionary<string, object> _storage = new Dictionary<string, object>();

        public void SetString(string key, string value) => _storage[key] = value;
        public string GetString(string key, string defaultValue = "") => _storage.ContainsKey(key) ? (string)_storage[key] : defaultValue;
        public void SetInt(string key, int value) => _storage[key] = value;
        public int GetInt(string key, int defaultValue = 0) => _storage.ContainsKey(key) ? (int)_storage[key] : defaultValue;
        public void SetBool(string key, bool value) => _storage[key] = value;
        public bool GetBool(string key, bool defaultValue = false) => _storage.ContainsKey(key) ? (bool)_storage[key] : defaultValue;
        public void SetObject<T>(string key, T obj) => _storage[key] = JsonUtils.ToJson(obj);
        public T GetObject<T>(string key, T defaultValue = default(T)) => _storage.ContainsKey(key) ? JsonUtils.FromJson<T>((string)_storage[key]) : defaultValue;
        public bool HasKey(string key) => _storage.ContainsKey(key);
        public void DeleteKey(string key) => _storage.Remove(key);
        public void DeleteAll() => _storage.Clear();
        public void Save() { }
    }
}