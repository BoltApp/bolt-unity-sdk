using System;

namespace BoltSDK
{
    /// <summary>
    /// Custom exception for Bolt SDK specific errors
    /// </summary>
    public class BoltSDKException : Exception
    {
        public string ErrorCode { get; }
        public string ErrorType { get; }

        public BoltSDKException(string message) : base(message)
        {
            ErrorType = "BoltSDKException";
        }

        public BoltSDKException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
            ErrorType = "BoltSDKException";
        }

        public BoltSDKException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorType = "BoltSDKException";
        }

        public BoltSDKException(string message, string errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
            ErrorType = "BoltSDKException";
        }
    }

    /// <summary>
    /// Exception thrown when the SDK is not initialized
    /// </summary>
    public class BoltSDKNotInitializedException : BoltSDKException
    {
        public BoltSDKNotInitializedException() : base("Bolt SDK is not initialized. Call Init() first.")
        {
            ErrorType = "BoltSDKNotInitializedException";
        }
    }

    /// <summary>
    /// Exception thrown when a product is not found in the catalog
    /// </summary>
    public class ProductNotFoundException : BoltSDKException
    {
        public ProductNotFoundException(string productId) : base($"Product with ID '{productId}' not found in catalog.")
        {
            ErrorType = "ProductNotFoundException";
        }
    }

    /// <summary>
    /// Exception thrown when web link operations fail
    /// </summary>
    public class WebLinkException : BoltSDKException
    {
        public WebLinkException(string message) : base(message)
        {
            ErrorType = "WebLinkException";
        }

        public WebLinkException(string message, Exception innerException) : base(message, innerException)
        {
            ErrorType = "WebLinkException";
        }
    }
}