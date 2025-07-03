using System.Collections;
using UnityEngine;

public class BoltApiService
{
    private readonly BoltClient client;

    /// <summary>
    /// Initializes a new instance of the <see cref="BoltApiService"/> class.
    /// </summary>
    /// <param name="baseUrl">The base URL of the your game server that implemented routes for the Bolt API.</param>
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
}
