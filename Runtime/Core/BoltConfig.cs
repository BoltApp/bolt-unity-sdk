using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoltApp
{
    /// <summary>
    /// Configuration class for Bolt SDK settings
    /// </summary>
    [Serializable]
    public class BoltConfig
    {
        public string gameId;
        public string publishableKey;
        public Environment environment = Environment.Development;

        // Track most recent payment links opened by the user. Used for analytic purposes only.
        // You should rely on the API for an up to date record of paid links.
        public int maxPaymentHistoryCount = 10;

        public BoltConfig()
        {
            gameId = "com.bolt.unity.test";
            publishableKey = "found_in.bolt.dashboard";
            environment = Environment.Development;
            maxPaymentHistoryCount = 10;
        }

        public BoltConfig(string gameId, string publishableKey, Environment environment, int maxPaymentHistoryCount = 10)
        {
            this.gameId = gameId;
            this.publishableKey = publishableKey;
            this.environment = environment;
            this.maxPaymentHistoryCount = maxPaymentHistoryCount;
        }

        public enum Environment
        {
            Development,
            Staging,
            Production
        }

        public string GetEnvironmentDisplayName()
        {
            switch (environment)
            {
                case Environment.Development:
                    return "Development";
                case Environment.Staging:
                    return "Staging";
                case Environment.Production:
                    return "Production";
                default:
                    return "Unknown";
            }
        }

        public bool IsProduction()
        {
            return environment == Environment.Production;
        }

        public bool IsDevelopment()
        {
            return environment == Environment.Development;
        }

        public bool IsStaging()
        {
            return environment == Environment.Staging;
        }
    }
}