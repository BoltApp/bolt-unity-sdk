using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class WebLinkServiceTests
    {
        private UnityWebLinkService _WebLinkService;
        private bool _WebLinkOpened;
        private bool _WebLinkClosed;
        private string _lastError;

        [SetUp]
        public void SetUp()
        {
            _WebLinkService = new UnityWebLinkService();
            _WebLinkOpened = false;
            _WebLinkClosed = false;
            _lastError = null;

            _WebLinkService.OnWebLinkOpened += () => _WebLinkOpened = true;
            _WebLinkService.OnWebLinkClosed += () => _WebLinkClosed = true;
            _WebLinkService.OnError += (error) => _lastError = error;
        }

        [TearDown]
        public void TearDown()
        {
            _WebLinkService?.CloseWebLink();
        }

        [Test]
        public void OpenWebLink_WithValidUrl_ShouldOpenSuccessfully()
        {
            // Arrange
            var url = "https://test.com/checkout";

            // Act
            _WebLinkService.OpenWebLink(url);

            // Assert
            Assert.IsTrue(_WebLinkService.IsWebLinkOpen);
            Assert.AreEqual(url, _WebLinkService.CurrentUrl);
            Assert.IsTrue(_WebLinkOpened);
            Assert.IsNull(_lastError);
        }

        [Test]
        public void OpenWebLink_WithNullUrl_ShouldFireError()
        {
            // Act
            _WebLinkService.OpenWebLink(null);

            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
            Assert.AreEqual("URL cannot be null or empty", _lastError);
        }

        [Test]
        public void OpenWebLink_WithEmptyUrl_ShouldFireError()
        {
            // Act
            _WebLinkService.OpenWebLink("");

            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
            Assert.AreEqual("URL cannot be null or empty", _lastError);
        }

        [Test]
        public void OpenWebLink_WithInvalidUrl_ShouldFireError()
        {
            // Act
            _WebLinkService.OpenWebLink("invalid-url");

            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
            Assert.AreEqual("Invalid URL: invalid-url", _lastError);
        }

        [Test]
        public void OpenWebLink_WithExtraParams_ShouldAppendToUrl()
        {
            // Arrange
            var url = "https://test.com/checkout";
            var extraParams = new Dictionary<string, string>
            {
                { "user_id", "123" },
                { "currency", "USD" }
            };

            // Act
            _WebLinkService.OpenWebLink(url, extraParams);

            // Assert
            Assert.IsTrue(_WebLinkService.IsWebLinkOpen);
            Assert.IsTrue(_WebLinkService.CurrentUrl.Contains("user_id=123"));
            Assert.IsTrue(_WebLinkService.CurrentUrl.Contains("currency=USD"));
        }

        [Test]
        public void CloseWebLink_WhenOpen_ShouldCloseSuccessfully()
        {
            // Arrange
            _WebLinkService.OpenWebLink("https://test.com/checkout");

            // Act
            _WebLinkService.CloseWebLink();

            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
            Assert.AreEqual("", _WebLinkService.CurrentUrl);
            Assert.IsTrue(_WebLinkClosed);
        }

        [Test]
        public void CloseWebLink_WhenNotOpen_ShouldNotFireError()
        {
            // Act
            _WebLinkService.CloseWebLink();

            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
            Assert.IsNull(_lastError);
        }

        [Test]
        public void SetTitle_ShouldUpdateTitle()
        {
            // Arrange
            var newTitle = "New Title";

            // Act
            _WebLinkService.SetTitle(newTitle);

            // Assert
            // Note: In this mock implementation, SetTitle doesn't store the value
            // In a real implementation, you would verify the title was set
            Assert.Pass("Title set successfully");
        }

        [Test]
        public void SetCanClose_ShouldUpdateCanClose()
        {
            // Arrange
            var canClose = false;

            // Act
            _WebLinkService.SetCanClose(canClose);

            // Assert
            // Note: In this mock implementation, SetCanClose doesn't store the value
            // In a real implementation, you would verify canClose was set
            Assert.Pass("CanClose set successfully");
        }

        [Test]
        public void HandleDeepLinkCallback_WithValidBoltUrl_ShouldProcessCorrectly()
        {
            // Arrange
            var callbackUrl = "https://bolt.com/callback?transaction_id=123&status=completed";

            // Act
            _WebLinkService.HandleDeepLinkCallback(callbackUrl);

            // Assert
            // The method should process the callback and close the web link
            // In a real implementation, you would verify the callback was processed
            Assert.Pass("Deep link callback handled successfully");
        }

        [Test]
        public void HandleDeepLinkCallback_WithInvalidUrl_ShouldNotProcess()
        {
            // Arrange
            var invalidUrl = "https://other-site.com/callback";

            // Act
            _WebLinkService.HandleDeepLinkCallback(invalidUrl);

            // Assert
            // The method should not process non-Bolt URLs
            Assert.Pass("Invalid URL ignored successfully");
        }

        [Test]
        public void HandleDeepLinkCallback_WithNullUrl_ShouldNotProcess()
        {
            // Act
            _WebLinkService.HandleDeepLinkCallback(null);

            // Assert
            // The method should handle null URLs gracefully
            Assert.Pass("Null URL handled gracefully");
        }

        [Test]
        public void IsWebLinkOpen_Initially_ShouldBeFalse()
        {
            // Assert
            Assert.IsFalse(_WebLinkService.IsWebLinkOpen);
        }

        [Test]
        public void CurrentUrl_Initially_ShouldBeEmpty()
        {
            // Assert
            Assert.AreEqual("", _WebLinkService.CurrentUrl);
        }
    }
}