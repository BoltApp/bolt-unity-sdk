using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKManagerInitializationTests
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
            result.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
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
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithNullConfig_ShouldFail()
        {
            // Act
            bool result = _sdkManager.Initialize(null!);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithInvalidUrl_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "not-a-valid-url",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithEmptyAppName_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = ""
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithNullAppName_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = null!
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithHttpUrl_ShouldSucceed()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "http://localhost:3000",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void Initialize_WithHttpsUrl_ShouldSucceed()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/v1",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void Initialize_WithUrlContainingQueryParameters_ShouldSucceed()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com?version=1.0",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void Initialize_WithUrlContainingPath_ShouldSucceed()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/api/v1",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void Initialize_WithFtpUrl_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "ftp://files.bolt.com",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_WithRelativeUrl_ShouldFail()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "/api/bolt",
                AppName = "TestApp"
            };

            // Act
            bool result = _sdkManager.Initialize(config);

            // Assert
            result.Should().BeFalse();
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void Initialize_MultipleTimes_ShouldWorkCorrectly()
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

            // Act
            bool result1 = _sdkManager.Initialize(config1);
            bool result2 = _sdkManager.Initialize(config2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            _sdkManager.IsInitialized.Should().BeTrue();
        }

        [Test]
        public void IsInitialized_WhenNotInitialized_ShouldReturnFalse()
        {
            // Act & Assert
            _sdkManager.IsInitialized.Should().BeFalse();
        }

        [Test]
        public void IsInitialized_WhenInitialized_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };

            // Act
            _sdkManager.Initialize(config);

            // Assert
            _sdkManager.IsInitialized.Should().BeTrue();
        }
    }
}