using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKConfigTests
    {
        [Test]
        public void Constructor_Default_ShouldCreateEmptyConfig()
        {
            // Act
            var config = new BoltSDKConfig();

            // Assert
            config.ServerUrl.Should().BeEmpty();
            config.AppName.Should().BeEmpty();
        }

        [Test]
        public void Constructor_WithParameters_ShouldSetProperties()
        {
            // Arrange
            string serverUrl = "https://api.bolt.com";
            string appName = "TestApp";

            // Act
            var config = new BoltSDKConfig(serverUrl, appName);

            // Assert
            config.ServerUrl.Should().Be(serverUrl);
            config.AppName.Should().Be(appName);
        }

        [Test]
        public void IsValid_WithValidConfig_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void IsValid_WithEmptyServerUrl_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithNullServerUrl_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = null!,
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithEmptyAppName_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = ""
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithNullAppName_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = null!
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithInvalidUrl_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "not-a-valid-url",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithHttpUrl_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "http://localhost:3000",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void IsValid_WithHttpsUrl_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/v1",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void IsValid_WithUrlContainingQueryParameters_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com?version=1.0",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void IsValid_WithUrlContainingPath_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/api/v1",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void IsValid_WithFtpUrl_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "ftp://files.bolt.com",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void IsValid_WithRelativeUrl_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "/api/bolt",
                AppName = "TestApp"
            };

            // Act
            bool isValid = config.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }
    }
}