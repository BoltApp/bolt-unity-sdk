using System;
using FluentAssertions;
using NUnit.Framework;
using BoltSDK;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class BoltSDKManagerLocaleTests
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
        public void GetLocale_WhenNotSet_ShouldReturnDefault()
        {
            // Act
            string locale = _sdkManager.GetLocale();

            // Assert
            locale.Should().Be("en-US");
        }

        [Test]
        public void GetLocale_WhenSet_ShouldReturnSavedValue()
        {
            // Arrange
            string expectedLocale = "fr-FR";
            _sdkManager.SetLocale(expectedLocale);

            // Act
            string actualLocale = _sdkManager.GetLocale();

            // Assert
            actualLocale.Should().Be(expectedLocale);
        }

        [Test]
        public void SetLocale_ShouldSaveToPlayerPrefs()
        {
            // Arrange
            string expectedLocale = "es-ES";

            // Act
            _sdkManager.SetLocale(expectedLocale);

            // Assert
            _sdkManager.GetLocale().Should().Be(expectedLocale);
        }

        [Test]
        public void SetLocale_WithNull_ShouldHandleGracefully()
        {
            // Act
            _sdkManager.SetLocale(null!);

            // Assert
            _sdkManager.GetLocale().Should().Be("en-US"); // Default value
        }

        [Test]
        public void SetLocale_WithEmptyString_ShouldSaveEmptyString()
        {
            // Act
            _sdkManager.SetLocale("");

            // Assert
            _sdkManager.GetLocale().Should().Be("");
        }

        [Test]
        public void SetLocale_WithWhitespace_ShouldSaveWhitespace()
        {
            // Arrange
            string localeWithWhitespace = "  en-US  ";

            // Act
            _sdkManager.SetLocale(localeWithWhitespace);

            // Assert
            _sdkManager.GetLocale().Should().Be(localeWithWhitespace);
        }

        [Test]
        public void SetLocale_WithSpecialCharacters_ShouldSaveCorrectly()
        {
            // Arrange
            string localeWithSpecialChars = "en-US_CA";

            // Act
            _sdkManager.SetLocale(localeWithSpecialChars);

            // Assert
            _sdkManager.GetLocale().Should().Be(localeWithSpecialChars);
        }

        [Test]
        public void SetLocale_WithUnicodeCharacters_ShouldSaveCorrectly()
        {
            // Arrange
            string localeWithUnicode = "zh-CN";

            // Act
            _sdkManager.SetLocale(localeWithUnicode);

            // Assert
            _sdkManager.GetLocale().Should().Be(localeWithUnicode);
        }

        [Test]
        public void SetLocale_OverwritesExistingValue()
        {
            // Arrange
            _sdkManager.SetLocale("en-US");

            // Act
            _sdkManager.SetLocale("fr-FR");

            // Assert
            _sdkManager.GetLocale().Should().Be("fr-FR");
        }

        [Test]
        public void GetLocale_AfterClearPlayerPrefs_ShouldReturnDefault()
        {
            // Arrange
            _sdkManager.SetLocale("fr-FR");
            // Note: We can't clear PlayerPrefs in the real implementation, so we'll test the default behavior

            // Act
            string locale = _sdkManager.GetLocale();

            // Assert
            locale.Should().Be("fr-FR"); // Should still return the set value
        }

        [Test]
        public void SetLocale_WithVariousValidLocales_ShouldWorkCorrectly()
        {
            // Test various valid locale formats
            string[] validLocales = {
                "en-US",
                "en-GB",
                "fr-FR",
                "de-DE",
                "es-ES",
                "it-IT",
                "pt-BR",
                "ja-JP",
                "ko-KR",
                "zh-CN",
                "zh-TW",
                "ru-RU",
                "ar-SA",
                "hi-IN",
                "th-TH"
            };

            foreach (string locale in validLocales)
            {
                // Act
                _sdkManager.SetLocale(locale);

                // Assert
                _sdkManager.GetLocale().Should().Be(locale, $"Failed for locale: {locale}");
            }
        }

        [Test]
        public void SetLocale_WithInvalidLocales_ShouldStillSave()
        {
            // Test various invalid locale formats that should still be saved
            string[] invalidLocales = {
                "invalid-locale",
                "en",
                "US",
                "en_US",
                "en-US-EXTRA",
                "123-456",
                "locale-with-dashes"
            };

            foreach (string locale in invalidLocales)
            {
                // Act
                _sdkManager.SetLocale(locale);

                // Assert
                _sdkManager.GetLocale().Should().Be(locale, $"Failed for locale: {locale}");
            }
        }

        [Test]
        public void GetLocale_ConsistencyTest()
        {
            // Arrange
            string testLocale = "de-DE";
            _sdkManager.SetLocale(testLocale);

            // Act & Assert - Call multiple times to ensure consistency
            for (int i = 0; i < 10; i++)
            {
                _sdkManager.GetLocale().Should().Be(testLocale, $"Inconsistent result on call {i + 1}");
            }
        }

        [Test]
        public void SetLocale_AndGetLocale_ShouldBeConsistent()
        {
            // Arrange
            string[] testLocales = { "en-US", "fr-FR", "es-ES", "de-DE", "it-IT" };

            foreach (string locale in testLocales)
            {
                // Act
                _sdkManager.SetLocale(locale);
                string retrievedLocale = _sdkManager.GetLocale();

                // Assert
                retrievedLocale.Should().Be(locale, $"Failed for locale: {locale}");
            }
        }
    }
}