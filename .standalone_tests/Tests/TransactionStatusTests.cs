using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class TransactionStatusTests
    {
        [Test]
        public void TransactionStatus_ShouldHaveFourValues()
        {
            // Act
            var values = System.Enum.GetValues<TransactionStatus>();

            // Assert
            values.Should().HaveCount(4);
        }

        [Test]
        public void TransactionStatus_ShouldHaveCorrectValues()
        {
            // Act & Assert
            ((int)TransactionStatus.Pending).Should().Be(0);
            ((int)TransactionStatus.Completed).Should().Be(1);
            ((int)TransactionStatus.Failed).Should().Be(2);
            ((int)TransactionStatus.Cancelled).Should().Be(3);
        }

        [Test]
        public void TransactionStatus_ToString_ShouldReturnCorrectString()
        {
            // Act & Assert
            TransactionStatus.Pending.ToString().Should().Be("Pending");
            TransactionStatus.Completed.ToString().Should().Be("Completed");
            TransactionStatus.Failed.ToString().Should().Be("Failed");
            TransactionStatus.Cancelled.ToString().Should().Be("Cancelled");
        }

        [Test]
        public void TransactionStatus_Parse_ShouldWorkCorrectly()
        {
            // Act & Assert
            System.Enum.Parse<TransactionStatus>("Pending").Should().Be(TransactionStatus.Pending);
            System.Enum.Parse<TransactionStatus>("Completed").Should().Be(TransactionStatus.Completed);
            System.Enum.Parse<TransactionStatus>("Failed").Should().Be(TransactionStatus.Failed);
            System.Enum.Parse<TransactionStatus>("Cancelled").Should().Be(TransactionStatus.Cancelled);
        }

        [Test]
        public void TransactionStatus_TryParse_ShouldWorkCorrectly()
        {
            // Act & Assert
            System.Enum.TryParse<TransactionStatus>("Pending", out var pending).Should().BeTrue();
            pending.Should().Be(TransactionStatus.Pending);

            System.Enum.TryParse<TransactionStatus>("Completed", out var completed).Should().BeTrue();
            completed.Should().Be(TransactionStatus.Completed);

            System.Enum.TryParse<TransactionStatus>("Failed", out var failed).Should().BeTrue();
            failed.Should().Be(TransactionStatus.Failed);

            System.Enum.TryParse<TransactionStatus>("Cancelled", out var cancelled).Should().BeTrue();
            cancelled.Should().Be(TransactionStatus.Cancelled);
        }

        [Test]
        public void TransactionStatus_TryParse_WithInvalidValue_ShouldReturnFalse()
        {
            // Act
            bool result = System.Enum.TryParse<TransactionStatus>("InvalidStatus", out var status);

            // Assert
            result.Should().BeFalse();
            status.Should().Be(TransactionStatus.Pending); // Default value
        }

        [Test]
        public void TransactionStatus_GetNames_ShouldReturnAllNames()
        {
            // Act
            var names = System.Enum.GetNames<TransactionStatus>();

            // Assert
            names.Should().Contain("Pending");
            names.Should().Contain("Completed");
            names.Should().Contain("Failed");
            names.Should().Contain("Cancelled");
            names.Should().HaveCount(4);
        }

        [Test]
        public void TransactionStatus_Comparison_ShouldWorkCorrectly()
        {
            // Act & Assert
            (TransactionStatus.Pending < TransactionStatus.Completed).Should().BeTrue();
            (TransactionStatus.Completed < TransactionStatus.Failed).Should().BeTrue();
            (TransactionStatus.Failed < TransactionStatus.Cancelled).Should().BeTrue();
            (TransactionStatus.Cancelled > TransactionStatus.Failed).Should().BeTrue();
        }
    }
}