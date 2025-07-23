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
        public string deepLinkAppName;
        public Environment environment = Environment.Development;

        public BoltConfig()
        {
            gameId = "com.bolt.unity.test";
            deepLinkAppName = "BoltAppTest";
            environment = Environment.Development;
        }

        public BoltConfig(string gameId, string deepLinkAppName, Environment environment)
        {
            this.gameId = gameId;
            this.deepLinkAppName = deepLinkAppName;
            this.environment = environment;
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