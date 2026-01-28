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

        public BoltConfig()
        {
            gameId = "com.bolt.unity.test";
            publishableKey = "found_in.bolt.dashboard";
            environment = Environment.Development;
        }

        public BoltConfig(string gameId, string publishableKey, Environment environment)
        {
            this.gameId = gameId;
            this.publishableKey = publishableKey;
            this.environment = environment;
        }

        public enum Environment
        {
            Development,
            Staging,
            Sandbox,
            Production
        }

        /// <summary>
        /// ad URL, per environment
        /// </summary>
        public string GetAdLink()
        {
            switch (environment)
            {
                case Environment.Development:
                case Environment.Staging:
                    return "https://play.staging-bolt.com";
                case Environment.Sandbox:
                    return "https://play.sandbox-bolt.com";
                case Environment.Production:
                default:
                    return "https://play.bolt.com";
            }
        }

        public string GetEnvironmentDisplayName()
        {
            switch (environment)
            {
                case Environment.Development:
                    return "Development";
                case Environment.Staging:
                    return "Staging";
                case Environment.Sandbox:
                    return "Sandbox";
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