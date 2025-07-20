using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKManagerUrlTests
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
            url.Should().StartWith("https://api.bolt.com");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
            url.Should().Contain($"locale={_sdkManager.GetLocale()}");
            url.Should().Contain($"app={config.AppName}");
        }

        [Test]
        public void BuildCheckoutUrl_WhenNotInitialized_ShouldReturnEmptyString()
        {
            // Arrange
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().BeEmpty();
        }

        [Test]
        public void BuildCheckoutUrl_WithNullProductId_ShouldReturnEmptyString()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(null!, transactionId);

            // Assert
            url.Should().BeEmpty();
        }

        [Test]
        public void BuildCheckoutUrl_WithEmptyProductId_ShouldReturnEmptyString()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl("", transactionId);

            // Assert
            url.Should().BeEmpty();
        }

        [Test]
        public void BuildCheckoutUrl_WithNullTransactionId_ShouldReturnEmptyString()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, null!);

            // Assert
            url.Should().BeEmpty();
        }

        [Test]
        public void BuildCheckoutUrl_WithEmptyTransactionId_ShouldReturnEmptyString()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, "");

            // Assert
            url.Should().BeEmpty();
        }

        [Test]
        public void BuildCheckoutUrl_WithSpecialCharacters_ShouldEscapeCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "Test App"
            };
            _sdkManager.Initialize(config);
            string productId = "product with spaces & symbols";
            string transactionId = "transaction-with-special-chars!@#$%";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().Contain("product=product%20with%20spaces%20%26%20symbols");
            url.Should().Contain("transaction=transaction-with-special-chars%21%40%23%24%25");
            url.Should().Contain("app=Test%20App");
        }

        [Test]
        public void BuildCheckoutUrl_WithUnicodeCharacters_ShouldEscapeCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "测试应用"
            };
            _sdkManager.Initialize(config);
            string productId = "产品名称";
            string transactionId = "交易ID-123";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().Contain("product=%E4%BA%A7%E5%93%81%E5%90%8D%E7%A7%B0");
            url.Should().Contain("transaction=%E4%BA%A4%E6%98%93ID-123");
            url.Should().Contain("app=%E6%B5%8B%E8%AF%95%E5%BA%94%E7%94%A8");
        }

        [Test]
        public void BuildCheckoutUrl_WithDifferentLocales_ShouldIncludeCorrectLocale()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            string[] locales = { "en-US", "fr-FR", "es-ES", "de-DE", "ja-JP" };

            foreach (string locale in locales)
            {
                // Arrange
                _sdkManager.SetLocale(locale);

                // Act
                string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

                // Assert
                url.Should().Contain($"locale={locale}", $"Failed for locale: {locale}");
            }
        }

        [Test]
        public void BuildCheckoutUrl_WithHttpServer_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "http://localhost:3000",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().StartWith("http://localhost:3000");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
        }

        [Test]
        public void BuildCheckoutUrl_WithHttpsServer_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/v1",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().StartWith("https://api.bolt.com/v1");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
        }

        [Test]
        public void BuildCheckoutUrl_WithServerContainingQueryParameters_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com?version=1.0",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().StartWith("https://api.bolt.com?version=1.0");
            url.Should().Contain("&product=");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
        }

        [Test]
        public void BuildCheckoutUrl_WithServerContainingPath_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com/api/v1",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().StartWith("https://api.bolt.com/api/v1");
            url.Should().Contain("?product=");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
        }

        [Test]
        public void BuildCheckoutUrl_WithVeryLongParameters_ShouldWorkCorrectly()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = new string('a', 100)
            };
            _sdkManager.Initialize(config);
            string productId = new string('b', 200);
            string transactionId = new string('c', 300);

            // Act
            string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url.Should().StartWith("https://api.bolt.com");
            url.Should().Contain($"product={productId}");
            url.Should().Contain($"transaction={transactionId}");
            url.Should().Contain($"app={config.AppName}");
        }

        [Test]
        public void BuildCheckoutUrl_ConsistencyTest()
        {
            // Arrange
            var config = new BoltSDKConfig
            {
                ServerUrl = "https://api.bolt.com",
                AppName = "TestApp"
            };
            _sdkManager.Initialize(config);
            string productId = "test-product";
            string transactionId = "test-transaction";

            // Act & Assert - Build URL multiple times to ensure consistency
            string firstUrl = _sdkManager.BuildCheckoutUrl(productId, transactionId);
            for (int i = 0; i < 10; i++)
            {
                string url = _sdkManager.BuildCheckoutUrl(productId, transactionId);
                url.Should().Be(firstUrl, $"Inconsistent URL on call {i + 1}");
            }
        }

        [Test]
        public void BuildCheckoutUrl_WithDifferentConfigs_ShouldProduceDifferentUrls()
        {
            // Arrange
            string productId = "test-product";
            string transactionId = "test-transaction";

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
            _sdkManager.Initialize(config1);
            string url1 = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            _sdkManager.Initialize(config2);
            string url2 = _sdkManager.BuildCheckoutUrl(productId, transactionId);

            // Assert
            url1.Should().NotBe(url2);
            url1.Should().Contain("TestApp1");
            url2.Should().Contain("TestApp2");
        }
    }
}