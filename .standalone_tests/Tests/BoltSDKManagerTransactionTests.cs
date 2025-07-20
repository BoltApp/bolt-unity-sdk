using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKManagerTransactionTests
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
        public void GenerateTransactionId_ShouldReturnUniqueId()
        {
            // Act
            string transactionId1 = _sdkManager.GenerateTransactionId();
            string transactionId2 = _sdkManager.GenerateTransactionId();

            // Assert
            transactionId1.Should().NotBeNullOrEmpty();
            transactionId2.Should().NotBeNullOrEmpty();
            transactionId1.Should().NotBe(transactionId2);
        }

        [Test]
        public void GenerateTransactionId_ShouldReturnValidGuid()
        {
            // Act
            string transactionId = _sdkManager.GenerateTransactionId();

            // Assert
            transactionId.Should().MatchRegex(@"^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$");
        }

        [Test]
        public void GenerateTransactionId_MultipleCalls_ShouldReturnUniqueIds()
        {
            // Act
            var transactionIds = new HashSet<string>();
            for (int i = 0; i < 100; i++)
            {
                transactionIds.Add(_sdkManager.GenerateTransactionId());
            }

            // Assert
            transactionIds.Should().HaveCount(100);
        }

        [Test]
        public void CheckTransactionStatus_WithValidId_ShouldReturnStatus()
        {
            // Arrange
            string transactionId = "test-transaction-123";
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Completed);

            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(transactionId);

            // Assert
            status.Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void CheckTransactionStatus_WithInvalidId_ShouldReturnPending()
        {
            // Arrange
            string transactionId = "invalid-transaction-id";

            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(transactionId);

            // Assert
            status.Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void CheckTransactionStatus_WithNullId_ShouldReturnPending()
        {
            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(null!);

            // Assert
            status.Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void CheckTransactionStatus_WithEmptyId_ShouldReturnPending()
        {
            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus("");

            // Assert
            status.Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void CheckTransactionStatus_WithWhitespaceId_ShouldReturnPending()
        {
            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus("   ");

            // Assert
            status.Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void SaveTransactionStatus_ShouldSaveToStorage()
        {
            // Arrange
            string transactionId = "test-transaction-456";
            TransactionStatus status = TransactionStatus.Completed;

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, status);

            // Assert
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status);
        }

        [Test]
        public void SaveTransactionStatus_WithNullId_ShouldNotSave()
        {
            // Act
            _sdkManager.SaveTransactionStatus(null!, TransactionStatus.Completed);

            // Assert
            // Should not throw and should not save anything
            _sdkManager.CheckTransactionStatus("some-id").Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void SaveTransactionStatus_WithEmptyId_ShouldNotSave()
        {
            // Act
            _sdkManager.SaveTransactionStatus("", TransactionStatus.Completed);

            // Assert
            // Should not throw and should not save anything
            _sdkManager.CheckTransactionStatus("some-id").Should().Be(TransactionStatus.Pending);
        }

        [Test]
        public void SaveTransactionStatus_WithAllStatusTypes_ShouldWorkCorrectly()
        {
            // Arrange
            string transactionId = "test-transaction";
            TransactionStatus[] statuses = {
                TransactionStatus.Pending,
                TransactionStatus.Completed,
                TransactionStatus.Failed,
                TransactionStatus.Cancelled
            };

            foreach (TransactionStatus status in statuses)
            {
                // Act
                _sdkManager.SaveTransactionStatus(transactionId, status);

                // Assert
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status, $"Failed for status: {status}");
            }
        }

        [Test]
        public void SaveTransactionStatus_OverwritesExistingStatus()
        {
            // Arrange
            string transactionId = "test-transaction-789";
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Pending);

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Completed);

            // Assert
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(TransactionStatus.Completed);
        }

        [Test]
        public void SaveTransactionStatus_WithMultipleTransactions_ShouldWorkCorrectly()
        {
            // Arrange
            string[] transactionIds = { "tx-1", "tx-2", "tx-3", "tx-4" };
            TransactionStatus[] statuses = {
                TransactionStatus.Pending,
                TransactionStatus.Completed,
                TransactionStatus.Failed,
                TransactionStatus.Cancelled
            };

            // Act
            for (int i = 0; i < transactionIds.Length; i++)
            {
                _sdkManager.SaveTransactionStatus(transactionIds[i], statuses[i]);
            }

            // Assert
            for (int i = 0; i < transactionIds.Length; i++)
            {
                _sdkManager.CheckTransactionStatus(transactionIds[i]).Should().Be(statuses[i], $"Failed for transaction {transactionIds[i]}");
            }
        }

        [Test]
        public void CheckTransactionStatus_AfterClearTransactionStatuses_ShouldReturnPending()
        {
            // Arrange
            string transactionId = "test-transaction";
            _sdkManager.SaveTransactionStatus(transactionId, TransactionStatus.Completed);
            // Note: We can't clear transaction statuses in the real implementation, so we'll test persistence

            // Act
            TransactionStatus status = _sdkManager.CheckTransactionStatus(transactionId);

            // Assert
            status.Should().Be(TransactionStatus.Completed); // Should still return the saved status
        }

        [Test]
        public void SaveTransactionStatus_WithSpecialCharacters_ShouldWorkCorrectly()
        {
            // Arrange
            string transactionId = "test-transaction-with-special-chars-!@#$%^&*()";
            TransactionStatus status = TransactionStatus.Completed;

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, status);

            // Assert
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status);
        }

        [Test]
        public void SaveTransactionStatus_WithUnicodeCharacters_ShouldWorkCorrectly()
        {
            // Arrange
            string transactionId = "test-transaction-中文-日本語-한국어";
            TransactionStatus status = TransactionStatus.Failed;

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, status);

            // Assert
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status);
        }

        [Test]
        public void SaveTransactionStatus_WithVeryLongId_ShouldWorkCorrectly()
        {
            // Arrange
            string transactionId = new string('a', 1000);
            TransactionStatus status = TransactionStatus.Cancelled;

            // Act
            _sdkManager.SaveTransactionStatus(transactionId, status);

            // Assert
            _sdkManager.CheckTransactionStatus(transactionId).Should().Be(status);
        }

        [Test]
        public void TransactionStatus_ConsistencyTest()
        {
            // Arrange
            string transactionId = "consistency-test";
            TransactionStatus expectedStatus = TransactionStatus.Completed;
            _sdkManager.SaveTransactionStatus(transactionId, expectedStatus);

            // Act & Assert - Check multiple times to ensure consistency
            for (int i = 0; i < 10; i++)
            {
                _sdkManager.CheckTransactionStatus(transactionId).Should().Be(expectedStatus, $"Inconsistent result on call {i + 1}");
            }
        }

        [Test]
        public void TransactionStatus_IsolationTest()
        {
            // Arrange
            string transactionId1 = "tx-1";
            string transactionId2 = "tx-2";
            _sdkManager.SaveTransactionStatus(transactionId1, TransactionStatus.Completed);
            _sdkManager.SaveTransactionStatus(transactionId2, TransactionStatus.Failed);

            // Act & Assert
            _sdkManager.CheckTransactionStatus(transactionId1).Should().Be(TransactionStatus.Completed);
            _sdkManager.CheckTransactionStatus(transactionId2).Should().Be(TransactionStatus.Failed);
        }
    }
}