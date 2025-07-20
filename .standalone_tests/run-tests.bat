@echo off
echo 🚀 Bolt Unity SDK - Standalone Test Suite
echo ==========================================
echo.

echo 📦 Restoring dependencies...
dotnet restore
if %errorlevel% neq 0 (
    echo ❌ Failed to restore dependencies
    pause
    exit /b 1
)

echo.
echo 🧪 Running tests...
dotnet test --verbosity normal
if %errorlevel% neq 0 (
    echo ❌ Some tests failed
    pause
    exit /b 1
)

echo.
echo ✅ All tests passed successfully!
echo.
pause 