using System.Collections;
using UnityEngine;

namespace BoltSDK
{
    public class BoltApiService
    {
        private readonly BoltClient client;

    public BoltApiService(string baseUrl)
    {
        this.client = new BoltClient(baseUrl);
    }

    public IEnumerator GetAllProducts(string pathOverride = null)
    {
        yield return client.Get(pathOverride ?? "products",
            res => Debug.Log($"Products: {res}"),
            err => Debug.LogError($"Error: {err}")
        );
    }

    public IEnumerator GetProduct(string productId, string pathOverride = null)
    {
        yield return client.Get(pathOverride ?? $"products/{productId}",
            res => Debug.Log($"Product: {res}"),
            err => Debug.LogError($"Error: {err}")
        );
    }

    public IEnumerator GetUserSubscriptions(string email, string pathOverride = null)
    {
        yield return client.Get(pathOverride ?? $"subscriptions/{email}",
            res => Debug.Log($"Subscriptions for {email}: {res}"),
            err => Debug.LogError($"Error: {err}")
        );
    }

    public IEnumerator GetSubscription(string subscriptionId, string pathOverride = null)
    {
        yield return client.Get(pathOverride ?? $"subscription/{subscriptionId}",
            res => Debug.Log($"Subscription {subscriptionId}: {res}"),
            err => Debug.LogError($"Error: {err}")
        );
    }

    public IEnumerator CancelSubscription(string subscriptionId)
    {
        yield return client.Delete($"subscription/{subscriptionId}",
            res => Debug.Log($"Canceled subscription {subscriptionId}: {res}"),
            err => Debug.LogError($"Error: {err}")
        );
    }

        // WebView integration method
        public void OpenPaymentWebView(string paymentUrl)
        {
            client.OpenPaymentWebView(paymentUrl);
        }

        public void OpenPaymentWebViewWithCallback(string paymentUrl, System.Action<string> onComplete, System.Action<string> onError)
        {
            client.OpenPaymentWebViewWithCallback(paymentUrl, onComplete, onError);
        }
    
    }
}
