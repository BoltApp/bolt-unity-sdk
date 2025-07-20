# Bolt Unity SDK - Standalone Test Suite

This directory contains a comprehensive test suite for the Bolt Unity SDK that can be run independently of Unity. The tests are designed to be modular, easy to read, and provide thorough coverage of all SDK functionality.

## 📁 Structure

```
.standalone_tests/
├── BoltSDK.StandaloneTests.sln    # Visual Studio/Rider solution file
├── BoltSDK.StandaloneTests.csproj # Project file with dependencies
├── README.md                      # This file
└── Tests/
    ├── BoltSDKConfigTests.cs      # Tests for configuration validation
    ├── TransactionStatusTests.cs   # Tests for transaction status enum
    ├── BoltSDKManagerInitializationTests.cs  # Tests for SDK initialization
    ├── BoltSDKManagerLocaleTests.cs          # Tests for locale management
    ├── BoltSDKManagerTransactionTests.cs     # Tests for transaction handling
    ├── BoltSDKManagerUrlTests.cs             # Tests for URL building
    ├── BoltSDKManagerDeepLinkTests.cs        # Tests for deep link processing
    ├── BoltSDKIntegrationTests.cs            # End-to-end integration tests
    ├── BoltSDKTestRunner.cs                  # Programmatic test runner
    └── Mocks/
        └── MockBoltSDKManager.cs             # Mock implementation for testing
```

## 🚀 Quick Start

### Prerequisites

- .NET 6.0 or later
- JetBrains Rider (recommended) or Visual Studio
- Git

### Running Tests

#### Option 1: Using Rider (Recommended)

1. Open `BoltSDK.StandaloneTests.sln` in Rider
2. Build the solution
3. Run all tests using the test runner or individual test files

#### Option 2: Using Command Line

```bash
# Navigate to the test directory
cd .standalone_tests

# Restore dependencies
dotnet restore

# Run all tests
dotnet test

# Run tests with verbose output
dotnet test --verbosity normal

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"
```

#### Option 3: Using Visual Studio

1. Open `BoltSDK.StandaloneTests.sln` in Visual Studio
2. Build the solution
3. Use Test Explorer to run tests

## 📋 Test Categories

### 1. Configuration Tests (`BoltSDKConfigTests.cs`)
- **Purpose**: Validate SDK configuration
- **Coverage**: URL validation, app name validation, edge cases
- **Tests**: 15+ tests covering various configuration scenarios

### 2. Transaction Status Tests (`TransactionStatusTests.cs`)
- **Purpose**: Test transaction status enum functionality
- **Coverage**: Enum values, parsing, string conversion
- **Tests**: 8+ tests covering enum behavior

### 3. Initialization Tests (`BoltSDKManagerInitializationTests.cs`)
- **Purpose**: Test SDK initialization process
- **Coverage**: Valid/invalid configs, multiple initializations
- **Tests**: 15+ tests covering initialization scenarios

### 4. Locale Management Tests (`BoltSDKManagerLocaleTests.cs`)
- **Purpose**: Test locale setting and retrieval
- **Coverage**: Various locales, edge cases, persistence
- **Tests**: 15+ tests covering locale functionality

### 5. Transaction Management Tests (`BoltSDKManagerTransactionTests.cs`)
- **Purpose**: Test transaction ID generation and status management
- **Coverage**: ID uniqueness, status persistence, edge cases
- **Tests**: 20+ tests covering transaction handling

### 6. URL Building Tests (`BoltSDKManagerUrlTests.cs`)
- **Purpose**: Test checkout URL generation
- **Coverage**: URL formatting, parameter encoding, edge cases
- **Tests**: 20+ tests covering URL building

### 7. Deep Link Tests (`BoltSDKManagerDeepLinkTests.cs`)
- **Purpose**: Test deep link processing
- **Coverage**: URL parsing, status updates, error handling
- **Tests**: 20+ tests covering deep link scenarios

### 8. Integration Tests (`BoltSDKIntegrationTests.cs`)
- **Purpose**: Test complete workflows
- **Coverage**: End-to-end scenarios, performance, edge cases
- **Tests**: 10+ tests covering integration scenarios

## 🔧 Test Features

### Mock Implementation
The test suite uses a `MockBoltSDKManager` that provides the same interface as the real SDK but without Unity dependencies. This allows for:
- Fast execution
- Deterministic results
- Easy debugging
- No Unity runtime required

### Comprehensive Coverage
- **Unit Tests**: Individual component testing
- **Integration Tests**: Complete workflow testing
- **Edge Case Tests**: Boundary conditions and error scenarios
- **Performance Tests**: Load testing and stress testing

### Fluent Assertions
All tests use FluentAssertions for readable and expressive assertions:
```csharp
result.Should().BeTrue();
url.Should().Contain("product=test");
status.Should().Be(TransactionStatus.Completed);
```

### Test Organization
- **Modular Files**: Each test file focuses on a specific component
- **Clear Naming**: Test methods follow the pattern `MethodName_Scenario_ExpectedResult`
- **Comprehensive Setup**: Each test is self-contained with proper setup/teardown

## 🎯 Test Scenarios Covered

### Configuration Validation
- ✅ Valid URLs (HTTP, HTTPS, with paths, with query params)
- ✅ Invalid URLs (relative, malformed, wrong schemes)
- ✅ Empty/null app names
- ✅ Edge cases and boundary conditions

### Transaction Management
- ✅ Unique transaction ID generation
- ✅ Status persistence and retrieval
- ✅ All transaction status types
- ✅ Special characters and Unicode
- ✅ Performance with large datasets

### URL Building
- ✅ Proper URL formatting
- ✅ Parameter encoding
- ✅ Locale inclusion
- ✅ Various server configurations
- ✅ Edge cases and error conditions

### Deep Link Processing
- ✅ Valid deep link parsing
- ✅ Invalid URL handling
- ✅ Status updates
- ✅ Error recovery
- ✅ Multiple transaction scenarios

### Integration Scenarios
- ✅ Complete initialization flow
- ✅ Transaction lifecycle
- ✅ Multiple concurrent operations
- ✅ Reset and reinitialize
- ✅ Performance testing

## 🚨 Error Handling

The test suite thoroughly tests error conditions:
- Invalid configurations
- Malformed URLs
- Missing parameters
- Network errors (simulated)
- Edge cases and boundary conditions

## 📊 Test Statistics

- **Total Test Files**: 8
- **Total Test Methods**: 150+
- **Coverage Areas**: 8 major components
- **Test Categories**: Unit, Integration, Performance, Edge Cases

## 🔍 Debugging

### Using Rider
1. Set breakpoints in test methods
2. Right-click on test and select "Debug"
3. Use the debug console to inspect variables

### Using Visual Studio
1. Set breakpoints in test methods
2. Right-click on test and select "Debug"
3. Use the debug window to inspect variables

### Programmatic Testing
```csharp
// Run all tests programmatically
var result = BoltSDKTestRunner.RunAllTests();
Console.WriteLine(result.ToString());

// Run specific test fixture
var fixtureResult = BoltSDKTestRunner.RunTestFixture(typeof(BoltSDKConfigTests));
Console.WriteLine(fixtureResult.ToString());
```

## 🛠️ Customization

### Adding New Tests
1. Create a new test file in the `Tests/` directory
2. Follow the naming convention: `ComponentNameTests.cs`
3. Use the existing test structure as a template
4. Add comprehensive test cases

### Modifying Test Behavior
1. Edit the `MockBoltSDKManager.cs` to simulate different scenarios
2. Add new test methods to existing test files
3. Update the test runner if needed

## 📈 Continuous Integration

The test suite is designed to work with CI/CD pipelines:
- Fast execution (no Unity runtime required)
- Deterministic results
- Clear pass/fail reporting
- Coverage reporting support

## 🤝 Contributing

When adding new tests:
1. Follow the existing naming conventions
2. Use FluentAssertions for assertions
3. Include comprehensive test cases
4. Add appropriate documentation
5. Ensure tests are self-contained

## 📝 Notes

- Tests are designed to be independent and can run in any order
- Each test cleans up after itself
- Mock implementation provides deterministic behavior
- All tests use descriptive names for easy debugging
- Performance tests are included for load testing scenarios

## 🎉 Success Criteria

A successful test run should show:
- ✅ All tests passing
- ✅ No warnings or errors
- ✅ Complete coverage of all SDK functionality
- ✅ Fast execution time
- ✅ Clear, readable test output 