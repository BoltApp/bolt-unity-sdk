using System;
using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class EditorTests
    {
        [Test]
        public void BoltConfig_CreateDefault_ShouldReturnValidConfig()
        {
            // Act
            var config = BoltConfig.CreateDefault();

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(BoltConfig.Environment.Development, config.environment);
            Assert.IsTrue(config.enableDebugMode);
            Assert.AreEqual(30, config.timeoutSeconds);
            Assert.AreEqual("Bolt Checkout", config.WebLinkTitle);
            Assert.IsTrue(config.allowWebLinkClose);
            Assert.IsTrue(config.enableAnalytics);
            Assert.AreEqual(3, config.maxRetryAttempts);
            Assert.AreEqual(2.0f, config.retryDelaySeconds);
            Assert.IsFalse(config.autoAcknowledgeTransactions);
        }

        [Test]
        public void BoltConfig_CreateForEnvironment_Development_ShouldReturnCorrectConfig()
        {
            // Act
            var config = BoltConfig.CreateForEnvironment(BoltConfig.Environment.Development);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(BoltConfig.Environment.Development, config.environment);
            Assert.IsTrue(config.enableDebugMode);
            Assert.AreEqual("https://dev-server.herokuapp.com", config.serverUrl);
        }

        [Test]
        public void BoltConfig_CreateForEnvironment_Staging_ShouldReturnCorrectConfig()
        {
            // Act
            var config = BoltConfig.CreateForEnvironment(BoltConfig.Environment.Staging);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(BoltConfig.Environment.Staging, config.environment);
            Assert.IsTrue(config.enableDebugMode);
            Assert.AreEqual("https://staging-server.herokuapp.com", config.serverUrl);
        }

        [Test]
        public void BoltConfig_CreateForEnvironment_Production_ShouldReturnCorrectConfig()
        {
            // Act
            var config = BoltConfig.CreateForEnvironment(BoltConfig.Environment.Production);

            // Assert
            Assert.IsNotNull(config);
            Assert.AreEqual(BoltConfig.Environment.Production, config.environment);
            Assert.IsFalse(config.enableDebugMode);
            Assert.AreEqual("https://prod-server.herokuapp.com", config.serverUrl);
        }

        [Test]
        public void BoltConfig_IsValid_WithValidConfig_ShouldReturnTrue()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                timeoutSeconds = 30,
                maxRetryAttempts = 3,
                retryDelaySeconds = 2.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsTrue(isValid);
        }

        [Test]
        public void BoltConfig_IsValid_WithMissingApiKey_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "",
                gameId = "test-game-id",
                timeoutSeconds = 30,
                maxRetryAttempts = 3,
                retryDelaySeconds = 2.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BoltConfig_IsValid_WithMissingGameId_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "",
                timeoutSeconds = 30,
                maxRetryAttempts = 3,
                retryDelaySeconds = 2.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BoltConfig_IsValid_WithInvalidTimeout_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                timeoutSeconds = 0,
                maxRetryAttempts = 3,
                retryDelaySeconds = 2.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BoltConfig_IsValid_WithInvalidRetryAttempts_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                timeoutSeconds = 30,
                maxRetryAttempts = -1,
                retryDelaySeconds = 2.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BoltConfig_IsValid_WithInvalidRetryDelay_ShouldReturnFalse()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                timeoutSeconds = 30,
                maxRetryAttempts = 3,
                retryDelaySeconds = -1.0f
            };

            // Act
            var isValid = config.IsValid();

            // Assert
            Assert.IsFalse(isValid);
        }

        [Test]
        public void BoltConfig_ToDictionary_ShouldReturnValidDictionary()
        {
            // Arrange
            var config = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                serverUrl = "https://test.com",
                environment = BoltConfig.Environment.Development,
                enableDebugMode = true,
                timeoutSeconds = 30,
                WebLinkTitle = "Test Checkout",
                allowWebLinkClose = true,
                enableAnalytics = true,
                analyticsEndpoint = "https://analytics.test.com",
                maxRetryAttempts = 3,
                retryDelaySeconds = 2.0f,
                autoAcknowledgeTransactions = false
            };

            // Act
            var dictionary = config.ToDictionary();

            // Assert
            Assert.IsNotNull(dictionary);
            Assert.AreEqual("test-api-key", dictionary["apiKey"]);
            Assert.AreEqual("test-game-id", dictionary["gameId"]);
            Assert.AreEqual("https://test.com", dictionary["serverUrl"]);
            Assert.AreEqual("Development", dictionary["environment"]);
            Assert.AreEqual(true, dictionary["enableDebugMode"]);
            Assert.AreEqual(30, dictionary["timeoutSeconds"]);
            Assert.AreEqual("Test Checkout", dictionary["WebLinkTitle"]);
            Assert.AreEqual(true, dictionary["allowWebLinkClose"]);
            Assert.AreEqual(true, dictionary["enableAnalytics"]);
            Assert.AreEqual("https://analytics.test.com", dictionary["analyticsEndpoint"]);
            Assert.AreEqual(3, dictionary["maxRetryAttempts"]);
            Assert.AreEqual(2.0f, dictionary["retryDelaySeconds"]);
            Assert.AreEqual(false, dictionary["autoAcknowledgeTransactions"]);
        }

        [Test]
        public void BoltConfig_Clone_ShouldReturnIdenticalConfig()
        {
            // Arrange
            var original = new BoltConfig
            {
                apiKey = "test-api-key",
                gameId = "test-game-id",
                serverUrl = "https://test.com",
                environment = BoltConfig.Environment.Production,
                enableDebugMode = false,
                timeoutSeconds = 60,
                WebLinkTitle = "Custom Checkout",
                allowWebLinkClose = false,
                enableAnalytics = false,
                analyticsEndpoint = "https://custom-analytics.com",
                maxRetryAttempts = 5,
                retryDelaySeconds = 3.0f,
                autoAcknowledgeTransactions = true
            };

            // Act
            var cloned = original.Clone();

            // Assert
            Assert.IsNotNull(cloned);
            Assert.AreEqual(original.apiKey, cloned.apiKey);
            Assert.AreEqual(original.gameId, cloned.gameId);
            Assert.AreEqual(original.serverUrl, cloned.serverUrl);
            Assert.AreEqual(original.environment, cloned.environment);
            Assert.AreEqual(original.enableDebugMode, cloned.enableDebugMode);
            Assert.AreEqual(original.timeoutSeconds, cloned.timeoutSeconds);
            Assert.AreEqual(original.WebLinkTitle, cloned.WebLinkTitle);
            Assert.AreEqual(original.allowWebLinkClose, cloned.allowWebLinkClose);
            Assert.AreEqual(original.enableAnalytics, cloned.enableAnalytics);
            Assert.AreEqual(original.analyticsEndpoint, cloned.analyticsEndpoint);
            Assert.AreEqual(original.maxRetryAttempts, cloned.maxRetryAttempts);
            Assert.AreEqual(original.retryDelaySeconds, cloned.retryDelaySeconds);
            Assert.AreEqual(original.autoAcknowledgeTransactions, cloned.autoAcknowledgeTransactions);
        }

        [Test]
        public void BoltConfig_Merge_ShouldMergeConfigurations()
        {
            // Arrange
            var original = new BoltConfig
            {
                apiKey = "original-api-key",
                gameId = "original-game-id",
                serverUrl = "https://original.com",
                environment = BoltConfig.Environment.Development
            };

            var other = new BoltConfig
            {
                apiKey = "new-api-key",
                gameId = "new-game-id",
                serverUrl = "https://new.com",
                environment = BoltConfig.Environment.Production,
                enableDebugMode = false
            };

            // Act
            original.Merge(other);

            // Assert
            Assert.AreEqual("new-api-key", original.apiKey);
            Assert.AreEqual("new-game-id", original.gameId);
            Assert.AreEqual("https://new.com", original.serverUrl);
            Assert.AreEqual(BoltConfig.Environment.Production, original.environment);
            Assert.IsFalse(original.enableDebugMode);
        }

        [Test]
        public void BoltConfig_GetEnvironmentDisplayName_ShouldReturnCorrectNames()
        {
            // Act & Assert
            var devConfig = new BoltConfig { environment = BoltConfig.Environment.Development };
            Assert.AreEqual("Development", devConfig.GetEnvironmentDisplayName());

            var stagingConfig = new BoltConfig { environment = BoltConfig.Environment.Staging };
            Assert.AreEqual("Staging", stagingConfig.GetEnvironmentDisplayName());

            var prodConfig = new BoltConfig { environment = BoltConfig.Environment.Production };
            Assert.AreEqual("Production", prodConfig.GetEnvironmentDisplayName());
        }

        [Test]
        public void BoltConfig_IsProduction_ShouldReturnCorrectValue()
        {
            // Arrange
            var devConfig = new BoltConfig { environment = BoltConfig.Environment.Development };
            var prodConfig = new BoltConfig { environment = BoltConfig.Environment.Production };

            // Act & Assert
            Assert.IsFalse(devConfig.IsProduction());
            Assert.IsTrue(prodConfig.IsProduction());
        }

        [Test]
        public void BoltConfig_IsDevelopment_ShouldReturnCorrectValue()
        {
            // Arrange
            var devConfig = new BoltConfig { environment = BoltConfig.Environment.Development };
            var prodConfig = new BoltConfig { environment = BoltConfig.Environment.Production };

            // Act & Assert
            Assert.IsTrue(devConfig.IsDevelopment());
            Assert.IsFalse(prodConfig.IsDevelopment());
        }

        [Test]
        public void BoltConfig_IsStaging_ShouldReturnCorrectValue()
        {
            // Arrange
            var stagingConfig = new BoltConfig { environment = BoltConfig.Environment.Staging };
            var prodConfig = new BoltConfig { environment = BoltConfig.Environment.Production };

            // Act & Assert
            Assert.IsTrue(stagingConfig.IsStaging());
            Assert.IsFalse(prodConfig.IsStaging());
        }
    }
}