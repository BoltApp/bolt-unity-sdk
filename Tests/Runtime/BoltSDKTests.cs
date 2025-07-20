using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using BoltSDK;

namespace BoltSDK.Tests
{
    public class BoltSDKTests
    {
        private BoltSDKManager _sdkManager;

        [SetUp]
        public void Setup()
        {
            _sdkManager = new BoltSDKManager();
        }

        [TearDown]
        public void Teardown()
        {
            _sdkManager = null;
        }

        [Test]
        public void Initialize_WithValidConfig_ShouldSucceed()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_sdkManager.IsInitialized);
        }

        [Test]
        public void Initialize_WithInvalidConfig_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "",
                AppName = ""
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(_sdkManager.IsInitialized);
        }

        [Test]
        public void GetLocale_ShouldReturnPlayerPrefsValue()
        {
            // Arrange
            string expectedLocale = "en-US";
            PlayerPrefs.SetString("BoltLocale", expectedLocale);

            // Act
            string actualLocale = _sdkManager.GetLocale();

            // Assert
            Assert.AreEqual(expectedLocale, actualLocale);
        }

        [Test]
        public void GetLocale_WhenNotSet_ShouldReturnDefault()
        {
            // Arrange
            PlayerPrefs.DeleteKey("BoltLocale");

            // Act
            string actualLocale = _sdkManager.GetLocale();

            // Assert
            Assert.AreEqual("en-US", actualLocale);
        }

        [Test]
        public void SetLocale_ShouldSaveToPlayerPrefs()
        {
            // Arrange
            string expectedLocale = "fr-FR";

            // Act
            _sdkManager.SetLocale(expectedLocale);

            // Assert
            Assert.AreEqual(expectedLocale, PlayerPrefs.GetString("BoltLocale"));
        }

        [Test]
        public void GenerateTransactionId_ShouldReturnUniqueId()
        {
            // Act
            string transactionId1 = _sdkManager.GenerateTransactionId();
            string transactionId2 = _sdkManager.GenerateTransactionId();

            // Assert
            Assert.IsNotNull(transactionId1);
            Assert.IsNotNull(transactionId2);
            Assert.AreNotEqual(transactionId1, transactionId2);
        }

        [Test]
        public void CheckTransactionStatus_WithValidId_ShouldReturnStatus()
        {
            // Arrange
            string transactionId = "test-transaction-123";
            PlayerPrefs.SetString($"BoltTransaction_{transactionId}", "completed");

            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(transactionId);

            // Assert
            Assert.AreEqual(TransactionStatus.Completed, status);
        }

        [Test]
        public void CheckTransactionStatus_WithInvalidId_ShouldReturnPending()
        {
            // Arrange
            string transactionId = "invalid-transaction-id";

            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(transactionId);

            // Assert
            Assert.AreEqual(TransactionStatus.Pending, status);
        }

        [Test]
        public void SaveTransactionStatus_ShouldSaveToPlayerPrefs()
        {
            // Arrange
            string transactionId = "test-transaction-456";
            TransactionStatus status = TransactionStatus.Completed;

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, status);

            // Assert
            string savedStatus = PlayerPrefs.GetString($"BoltTransaction_{transactionId}");
            Assert.AreEqual("completed", savedStatus);
        }

        [Test]
        public void BuildCheckoutUrl_WithValidParameters_ShouldReturnValidUrl()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction-789";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            Assert.IsTrue(url.StartsWith("https://api.bolt.com"));
            Assert.IsTrue(url.Contains($"product={productId}"));
            Assert.IsTrue(url.Contains($"transaction={transactionId}"));
            Assert.IsTrue(url.Contains($"locale={_sdkManager.GetLocale()}"));
        }

        [UnityTest]
        public IEnumerator HandleDeepLink_WithValidUrl_ShouldProcessTransaction()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string transactionId = "test-transaction-123";
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={transactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            Assert.IsTrue(result);
            yield return null; // Wait one frame
            Assert.AreEqual(TransactionStatus.Completed, _sdkManager.CheckTransactionStatus(transactionId));
        }

        [Test]
        public void HandleDeepLink_WithInvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            string invalidUrl = "invalid-url";

            // Act
            bool result = _sdkManager.HandleDeepLink(invalidUrl);

            // Assert
            Assert.IsFalse(result);
        }
    }
}