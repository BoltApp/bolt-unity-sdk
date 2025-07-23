using System;
using System.Collections.Generic;
using UnityEngine;

namespace BoltSDK
{
    /// <summary>
    /// Configuration class for Bolt SDK settings
    /// </summary>
    [Serializable]
    public class BoltConfig
    {
        [Header("API Configuration")]
        public string gameId;
        public string deepLinkAppName;

        [Header("Environment Settings")]
        public Environment environment = Environment.Development;


        /// <summary>
        /// Environment types for different deployment stages
        /// </summary>
        public enum Environment
        {
            Development,
            Staging,
            Production
        }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsValid()
        {
            if (string.IsNullOrEmpty(gameId))
            {
                Debug.LogError("BoltConfig: Game ID is required");
                return false;
            }

            if (string.IsNullOrEmpty(deepLinkAppName))
            {
                Debug.LogError("BoltConfig: Deep link app name is required");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets a display name for the environment
        /// </summary>
        /// <returns>Human-readable environment name</returns>
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

        /// <summary>
        /// Checks if this is a production environment
        /// </summary>
        /// <returns>True if production, false otherwise</returns>
        public bool IsProduction()
        {
            return environment == Environment.Production;
        }

        /// <summary>
        /// Checks if this is a development environment
        /// </summary>
        /// <returns>True if development, false otherwise</returns>
        public bool IsDevelopment()
        {
            return environment == Environment.Development;
        }

        /// <summary>
        /// Checks if this is a staging environment
        /// </summary>
        /// <returns>True if staging, false otherwise</returns>
        public bool IsStaging()
        {
            return environment == Environment.Staging;
        }
    }
}