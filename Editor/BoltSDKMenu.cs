using UnityEngine;
using UnityEditor;
using BoltSDK;

namespace BoltSDK.Editor
{
    /// <summary>
    /// Editor menu items for Bolt SDK
    /// </summary>
    public static class BoltSDKMenu
    {
        private const string MENU_ROOT = "Tools/Bolt SDK/";
        private const string CONFIG_PATH = "Assets/BoltConfig.asset";

        [MenuItem(MENU_ROOT + "Configuration", false, 100)]
        public static void OpenConfiguration()
        {
            BoltConfigWindow.ShowWindow();
        }

        [MenuItem(MENU_ROOT + "Documentation", false, 600)]
        public static void OpenDocumentation()
        {
            Application.OpenURL("https://docs.bolt.com/unity-sdk");
        }

        [MenuItem(MENU_ROOT + "Discord Support", false, 700)]
        public static void OpenDiscord()
        {
            Application.OpenURL("https://discord.gg/BSUp9qjtnc");
        }

        [MenuItem(MENU_ROOT + "About Bolt SDK", false, 800)]
        public static void ShowAbout()
        {
            EditorUtility.DisplayDialog("Bolt SDK",
                "Bolt Unity SDK v0.0.1\n\n" +
                "A Unity SDK for performing in-app purchases and subscriptions outside of the app store.\n\n" +
                "For support, visit our Discord: https://discord.gg/BSUp9qjtnc\n\n" +
                "Documentation: https://docs.bolt.com/unity-sdk",
                "OK");
        }
    }
}