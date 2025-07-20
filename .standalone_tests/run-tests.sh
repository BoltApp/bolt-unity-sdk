#!/bin/bash

echo "🚀 Bolt Unity SDK - Standalone Test Suite"
echo "=========================================="
echo

echo "📦 Restoring dependencies..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "❌ Failed to restore dependencies"
    exit 1
fi

echo
echo "🧪 Running tests..."
dotnet test --verbosity normal
if [ $? -ne 0 ]; then
    echo "❌ Some tests failed"
    exit 1
fi

echo
echo "✅ All tests passed successfully!"
echo 