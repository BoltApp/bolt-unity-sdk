using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace BoltSDK.Tests
{
    [TestFixture]
    public class StorageServiceTests
    {
        private PlayerPrefsStorageService _storageService;

        [SetUp]
        public void SetUp()
        {
            _storageService = new PlayerPrefsStorageService();
            // Clear any existing data
            _storageService.DeleteAll();
        }

        [TearDown]
        public void TearDown()
        {
            _storageService?.DeleteAll();
        }

        [Test]
        public void SetString_AndGetString_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test_string_key";
            var value = "test_string_value";

            // Act
            _storageService.SetString(key, value);
            var retrievedValue = _storageService.GetString(key);

            // Assert
            Assert.AreEqual(value, retrievedValue);
        }

        [Test]
        public void GetString_WithNonExistentKey_ShouldReturnDefaultValue()
        {
            // Arrange
            var key = "non_existent_key";
            var defaultValue = "default_value";

            // Act
            var result = _storageService.GetString(key, defaultValue);

            // Assert
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void SetInt_AndGetInt_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test_int_key";
            var value = 42;

            // Act
            _storageService.SetInt(key, value);
            var retrievedValue = _storageService.GetInt(key);

            // Assert
            Assert.AreEqual(value, retrievedValue);
        }

        [Test]
        public void GetInt_WithNonExistentKey_ShouldReturnDefaultValue()
        {
            // Arrange
            var key = "non_existent_int_key";
            var defaultValue = 100;

            // Act
            var result = _storageService.GetInt(key, defaultValue);

            // Assert
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void SetBool_AndGetBool_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test_bool_key";
            var value = true;

            // Act
            _storageService.SetBool(key, value);
            var retrievedValue = _storageService.GetBool(key);

            // Assert
            Assert.AreEqual(value, retrievedValue);
        }

        [Test]
        public void GetBool_WithNonExistentKey_ShouldReturnDefaultValue()
        {
            // Arrange
            var key = "non_existent_bool_key";
            var defaultValue = true;

            // Act
            var result = _storageService.GetBool(key, defaultValue);

            // Assert
            Assert.AreEqual(defaultValue, result);
        }

        [Test]
        public void SetObject_AndGetObject_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "test_object_key";
            var testObject = new BoltUser
            {
                Email = "test@example.com",
                Locale = "en-US",
                Country = "US"
            };

            // Act
            _storageService.SetObject(key, testObject);
            var retrievedObject = _storageService.GetObject<BoltUser>(key);

            // Assert
            Assert.IsNotNull(retrievedObject);
            Assert.AreEqual(testObject.Email, retrievedObject.Email);
            Assert.AreEqual(testObject.Locale, retrievedObject.Locale);
            Assert.AreEqual(testObject.Country, retrievedObject.Country);
        }

        [Test]
        public void GetObject_WithNonExistentKey_ShouldReturnDefaultValue()
        {
            // Arrange
            var key = "non_existent_object_key";
            var defaultValue = new BoltUser { Email = "default@example.com" };

            // Act
            var result = _storageService.GetObject(key, defaultValue);

            // Assert
            Assert.AreEqual(defaultValue.Email, result.Email);
        }

        [Test]
        public void HasKey_WithExistingKey_ShouldReturnTrue()
        {
            // Arrange
            var key = "existing_key";
            _storageService.SetString(key, "value");

            // Act
            var hasKey = _storageService.HasKey(key);

            // Assert
            Assert.IsTrue(hasKey);
        }

        [Test]
        public void HasKey_WithNonExistentKey_ShouldReturnFalse()
        {
            // Arrange
            var key = "non_existent_key";

            // Act
            var hasKey = _storageService.HasKey(key);

            // Assert
            Assert.IsFalse(hasKey);
        }

        [Test]
        public void DeleteKey_ShouldRemoveKey()
        {
            // Arrange
            var key = "key_to_delete";
            _storageService.SetString(key, "value");
            Assert.IsTrue(_storageService.HasKey(key));

            // Act
            _storageService.DeleteKey(key);

            // Assert
            Assert.IsFalse(_storageService.HasKey(key));
        }

        [Test]
        public void DeleteAll_ShouldRemoveAllKeys()
        {
            // Arrange
            _storageService.SetString("key1", "value1");
            _storageService.SetString("key2", "value2");
            _storageService.SetInt("key3", 42);

            // Act
            _storageService.DeleteAll();

            // Assert
            Assert.IsFalse(_storageService.HasKey("key1"));
            Assert.IsFalse(_storageService.HasKey("key2"));
            Assert.IsFalse(_storageService.HasKey("key3"));
        }

        [Test]
        public void Save_ShouldNotThrowException()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _storageService.Save());
        }

        [Test]
        public void SetString_WithNullValue_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "null_value_key";

            // Act
            _storageService.SetString(key, null);
            var result = _storageService.GetString(key);

            // Assert
            Assert.AreEqual("", result);
        }

        [Test]
        public void SetObject_WithNullObject_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "null_object_key";

            // Act
            _storageService.SetObject<BoltUser>(key, null);
            var result = _storageService.GetObject<BoltUser>(key);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void GetString_WithEmptyKey_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "";
            var value = "test_value";

            // Act
            _storageService.SetString(key, value);
            var result = _storageService.GetString(key);

            // Assert
            Assert.AreEqual(value, result);
        }

        [Test]
        public void SetObject_WithComplexObject_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "complex_object_key";
            var catalog = new Catalog("test_product", "Test Product", "Test Description", 9.99m, "USD", "https://test.com")
            {
                Category = "Test Category",
                IsActive = true
            };

            // Act
            _storageService.SetObject(key, catalog);
            var retrievedCatalog = _storageService.GetObject<Catalog>(key);

            // Assert
            Assert.IsNotNull(retrievedCatalog);
            Assert.AreEqual(catalog.ProductId, retrievedCatalog.ProductId);
            Assert.AreEqual(catalog.Name, retrievedCatalog.Name);
            Assert.AreEqual(catalog.Price, retrievedCatalog.Price);
            Assert.AreEqual(catalog.Currency, retrievedCatalog.Currency);
            Assert.AreEqual(catalog.Category, retrievedCatalog.Category);
            Assert.AreEqual(catalog.IsActive, retrievedCatalog.IsActive);
        }

        [Test]
        public void SetObject_WithArray_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "array_key";
            var array = new[] { "item1", "item2", "item3" };

            // Act
            _storageService.SetObject(key, array);
            var retrievedArray = _storageService.GetObject<string[]>(key);

            // Assert
            Assert.IsNotNull(retrievedArray);
            Assert.AreEqual(array.Length, retrievedArray.Length);
            for (int i = 0; i < array.Length; i++)
            {
                Assert.AreEqual(array[i], retrievedArray[i]);
            }
        }

        [Test]
        public void SetObject_WithDictionary_ShouldWorkCorrectly()
        {
            // Arrange
            var key = "dictionary_key";
            var dictionary = new Dictionary<string, object>
            {
                { "key1", "value1" },
                { "key2", 42 },
                { "key3", true }
            };

            // Act
            _storageService.SetObject(key, dictionary);
            var retrievedDictionary = _storageService.GetObject<Dictionary<string, object>>(key);

            // Assert
            Assert.IsNotNull(retrievedDictionary);
            Assert.AreEqual(dictionary.Count, retrievedDictionary.Count);
            Assert.AreEqual(dictionary["key1"], retrievedDictionary["key1"]);
            Assert.AreEqual(dictionary["key2"], retrievedDictionary["key2"]);
            Assert.AreEqual(dictionary["key3"], retrievedDictionary["key3"]);
        }
    }
}