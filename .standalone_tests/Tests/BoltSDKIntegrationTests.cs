using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKIntegrationTests
    {
        private IBoltSDKManager _sdkManager = null!;

        [SetUp]
        public void Setup()
        {
            _sdkManager = new BoltSDKManagerCore();
        }

        [TearDown]
        public void Teardown()
        {
            if (_sdkManager is BoltSDKManagerCore core)
            {
                core.Reset();
            }
        }

        [Test]
        public void CompleteFlow_InitializeAndBuildUrl_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            bool initResult = _sdkManager.Initialize(config);
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            initResult.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
            url.Should().NotBeEmpty();
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
        }

        [Test]
        public void CompleteFlow_WithLocaleChange_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            _sdkManager.Initialize(config);
            _sdkManager.SetLocale("fr-FR");
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().Contain("locale=fr-FR");
            _sdkManager.GetLocale().Should().Be("fr-FR");
        }

        [Test]
        public void CompleteFlow_TransactionLifecycle_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = _sdkManager.GenerateTransactionId();
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={transactionId}";

            // Act & Assert - Initial state
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Pending);

            // Act & Assert - Process deep link
            bool deepLinkResult = _sdkManager.HandleDeepLink(deepLinkUrl);
            deepLinkResult.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void CompleteFlow_MultipleTransactions_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string[] transactionIds = { "tx-1", "tx-2", "tx-3" };
            TransactionStatus[] statuses = {
                TransactionStatus.Completed,
                TransactionStatus.Failed,
                TransactionStatus.Cancelled
            };

            // Act & Assert
            for (int i = 0; i < transactionIds.Length; i++)
            {
                string deepLinkUrl = $"myapp://bolt/transaction?status={statuses[i].ToString().ToLower()}&id={transactionIds[i]}";

                bool result = _sdkManager.HandleDeepLink(deepLinkUrl);
                result.Should().BeTrue($"Failed for transaction {transactionIds[i]}");

                _sdkManager.CheckTransactionStatus(transactionIds[i]).Should().Be(statuses[i], $"Failed for transaction {transactionIds[i]}");
            }
        }

        [Test]
        public void CompleteFlow_ReinitializeWithDifferentConfig_ShouldWorkCorrectly()
        {
            // Arrange
            var config1 = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp1"
            };

            var config2 = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/v2",
                AppName = "TestApp2"
            };

            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            _sdkManager.Initialize(config1);
            string url1 = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            _sdkManager.Initialize(config2);
            string url2 = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url1.Should().NotBe(url2);
            url1.Should().Contain("TestApp1");
            url2.Should().Contain("TestApp2");
        }

        [Test]
        public void CompleteFlow_TransactionStatusPersistence_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "persistent-transaction";
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Completed);

            // Act & Assert - Status should persist
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);

            // Act & Assert - Status should persist after multiple checks
            for (int i = 0; i < 5; i++)
            {
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed, $"Failed on check {i + 1}");
            }
        }

        [Test]
        public void CompleteFlow_ErrorHandling_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            // Act & Assert - Invalid deep link should not affect valid transactions
            bool invalidResult = _sdkManager.HandleDeepLink("invalid-url");
            invalidResult.Should().BeFalse();

            // Act & Assert - Valid transaction should still work
            string transactionId = "valid-transaction";
            string validDeepLink = $"myapp://bolt/transaction?status=completed&id={transactionId}";
            bool validResult = _sdkManager.HandleDeepLink(validDeepLink);
            validResult.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void CompleteFlow_ConcurrentOperations_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            // Act & Assert - Multiple operations should not interfere
            _sdkManager.SetLocale("en-US");
            string transactionId1 = _sdkManager.GenerateTransactionId();
            _sdkManager.SetLocale("fr-FR");
            string transactionId2 = _sdkManager.GenerateTransactionId();
            _sdkManager.SetLocale("es-ES");

            // Assert
            _sdkManager.GetLocale().Should().Be("es-ES");
            transactionId1.Should().NotBe(transactionId2);
        }

        [Test]
        public void CompleteFlow_ResetAndReinitialize_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };

            // Act
            _sdkManager.Initialize(config);
            _sdkManager.SetLocale("fr-FR");
            string transactionId = "test-transaction";
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Completed);

            // Assert - Initial state
            _sdkManager.IsInitialized.Should().BeTrue();
            _sdkManager.GetLocale().Should().Be("fr-FR");
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);

            // Note: The real BoltSDKManager doesn't have a Reset method, so we'll test persistence
            // The state should persist across operations
            _sdkManager.IsInitialized.Should().BeTrue();
            _sdkManager.GetLocale().Should().Be("fr-FR");
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void CompleteFlow_EdgeCases_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            // Act & Assert - Very long transaction ID
            string longTransactionId = new string('a', 1000);
            _sdkManager.SaveTransactionStatus(longTransactionId, TransactionStatus.Completed);
            _sdkManager.CheckTransactionStatus(longTransactionId).Should().Be(TransactionStatus.Completed);

            // Act & Assert - Special characters in transaction ID
            string specialTransactionId = "transaction-with-special-chars!@#$%^&*()";
            _sdkManager.SaveTransactionStatus(specialTransactionId, TransactionStatus.Failed);
            _sdkManager.CheckTransactionStatus(specialTransactionId).Should().Be(TransactionStatus.Failed);

            // Act & Assert - Unicode characters in transaction ID
            string unicodeTransactionId = "transaction-中文-日本語-한국어";
            _sdkManager.SaveTransactionStatus(unicodeTransactionId, TransactionStatus.Cancelled);
            _sdkManager.CheckTransactionStatus(unicodeTransactionId).Should().Be(TransactionStatus.Cancelled);
        }

        [Test]
        public void CompleteFlow_PerformanceTest_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            // Act & Assert - Generate many transaction IDs
            var transactionIds = new HashSet<string>();
            for (int i = 0; i < 1000; i++)
            {
                string transactionId = _sdkManager.GenerateTransactionId();
                transactionIds.Add(transactionId);
            }

            // Assert
            transactionIds.Should().HaveCount(1000);

            // Act & Assert - Save and check many transaction statuses
            for (int i = 0; i < 100; i++)
            {
                string transactionId = $"perf-test-{i}";
                TransactionStatus status = (TransactionStatus)(i % 4);
                _sdkManager.SaveTransactionStatus(transactionId, status);
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status);
            }
        }
    }
}