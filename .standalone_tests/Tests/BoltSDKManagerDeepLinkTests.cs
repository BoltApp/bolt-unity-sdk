using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKManagerDeepLinkTests
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
        public void HandleDeepLink_WithValidUrl_ShouldProcessTransaction()
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
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithInvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            string invalidUrl = "invalid-url";

            // Act
            bool result = _sdkManager.HandleDeepLink(invalidUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithNullUrl_ShouldReturnFalse()
        {
            // Act
            bool result = _sdkManager.HandleDeepLink(null!);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithEmptyUrl_ShouldReturnFalse()
        {
            // Act
            bool result = _sdkManager.HandleDeepLink("");

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithWrongScheme_ShouldReturnFalse()
        {
            // Arrange
            string wrongSchemeUrl = "https://bolt.com/transaction?status=completed&id=123";

            // Act
            bool result = _sdkManager.HandleDeepLink(wrongSchemeUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithWrongHost_ShouldReturnFalse()
        {
            // Arrange
            string wrongHostUrl = "myapp://other.com/transaction?status=completed&id=123";

            // Act
            bool result = _sdkManager.HandleDeepLink(wrongHostUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithMissingTransactionId_ShouldReturnFalse()
        {
            // Arrange
            string missingIdUrl = "myapp://bolt/transaction?status=completed";

            // Act
            bool result = _sdkManager.HandleDeepLink(missingIdUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithEmptyTransactionId_ShouldReturnFalse()
        {
            // Arrange
            string emptyIdUrl = "myapp://bolt/transaction?status=completed&id=";

            // Act
            bool result = _sdkManager.HandleDeepLink(emptyIdUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithAllStatusTypes_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string[] statuses = { "completed", "failed", "cancelled", "pending" };
            TransactionStatus[] expectedStatuses = {
                TransactionStatus.Completed,
                TransactionStatus.Failed,
                TransactionStatus.Cancelled,
                TransactionStatus.Pending
            };

            for (int i = 0; i < statuses.Length; i++)
            {
                string transactionId = $"test-transaction-{i}";
                string deepLinkUrl = $"myapp://bolt/transaction?status={statuses[i]}&id={transactionId}";

                // Act
                bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

                // Assert
                result.Should().BeTrue($"Failed for status: {statuses[i]}");
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(expectedStatuses[i], $"Failed for status: {statuses[i]}");
            }
        }

        [Test]
        public void HandleDeepLink_WithCaseInsensitiveStatus_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-case";
            string deepLinkUrl = $"myapp://bolt/transaction?status=COMPLETED&id={transactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithUnknownStatus_ShouldDefaultToPending()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-unknown";
            string deepLinkUrl = $"myapp://bolt/transaction?status=unknown&id={transactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void HandleDeepLink_WithMissingStatus_ShouldDefaultToPending()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-no-status";
            string deepLinkUrl = $"myapp://bolt/transaction?id={transactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void HandleDeepLink_WithSpecialCharactersInTransactionId_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "transaction-with-special-chars!@#$%";
            string encodedTransactionId = Uri.EscapeDataString(transactionId);
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={encodedTransactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithUnicodeCharactersInTransactionId_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "transaction-中文-日本語-한국어";
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={transactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithUrlEncodedParameters_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "transaction with spaces";
            string encodedTransactionId = "transaction%20with%20spaces";
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={encodedTransactionId}";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithMultipleQueryParameters_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-multi";
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={transactionId}&extra=param&another=value";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithReorderedQueryParameters_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-reorder";
            string deepLinkUrl = $"myapp://bolt/transaction?id={transactionId}&status=completed";

            // Act
            bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

            // Assert
            result.Should().BeTrue();
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void HandleDeepLink_WithMalformedUrl_ShouldReturnFalse()
        {
            // Arrange
            string malformedUrl = "myapp://bolt/transaction?status=completed&id=123%";

            // Act
            bool result = _sdkManager.HandleDeepLink(malformedUrl);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_WithInvalidUri_ShouldReturnFalse()
        {
            // Arrange
            string invalidUri = "myapp://bolt/transaction?status=completed&id=123%";

            // Act
            bool result = _sdkManager.HandleDeepLink(invalidUri);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public void HandleDeepLink_ConsistencyTest()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string transactionId = "test-transaction-consistency";
            string deepLinkUrl = $"myapp://bolt/transaction?status=completed&id={transactionId}";

            // Act & Assert - Process the same deep link multiple times
            for (int i = 0; i < 5; i++)
            {
                bool result = _sdkManager.HandleDeepLink(deepLinkUrl);
                result.Should().BeTrue($"Failed on iteration {i + 1}");
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed, $"Failed on iteration {i + 1}");
            }
        }

        [Test]
        public void HandleDeepLink_WithDifferentTransactionIds_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);

            string[] transactionIds = { "tx-1", "tx-2", "tx-3", "tx-4" };
            TransactionStatus[] statuses = {
                TransactionStatus.Completed,
                TransactionStatus.Failed,
                TransactionStatus.Cancelled,
                TransactionStatus.Pending
            };

            for (int i = 0; i < transactionIds.Length; i++)
            {
                string deepLinkUrl = $"myapp://bolt/transaction?status={statuses[i].ToString().ToLower()}&id={transactionIds[i]}";

                // Act
                bool result = _sdkManager.HandleDeepLink(deepLinkUrl);

                // Assert
                result.Should().BeTrue($"Failed for transaction {transactionIds[i]}");
                _sdkManager.CheckTransactionStatus(transactionIds[i]).Should().Be(statuses[i], $"Failed for transaction {transactionIds[i]}");
            }
        }
    }
}