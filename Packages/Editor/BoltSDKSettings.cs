using UnityEngine;
using UnityEditor;

namespace BoltSDK.Editor
{
    public class BoltSDKSettings : EditorWindow
    {
        private string _serverUrl = "";
        private string _appName = "";
        private Vector2 _scrollPosition;

        [MenuItem("Tools/Bolt SDK/Settings")]
        public static void ShowWindow()
        {
            BoltSDKSettings window = GetWindow<BoltSDKSettings>("Bolt SDK Settings");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void LoadSettings()
        {
            _serverUrl = EditorPrefs.GetString("BoltSDK_ServerUrl", "");
            _appName = EditorPrefs.GetString("BoltSDK_AppName", "");
        }

        private void SaveSettings()
        {
            EditorPrefs.SetString("BoltSDK_ServerUrl", _serverUrl);
            EditorPrefs.SetString("BoltSDK_AppName", _appName);
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.Space(10);

            // Header
            EditorGUILayout.LabelField("Bolt SDK Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(5);

            EditorGUILayout.HelpBox("Configure your Bolt SDK settings. These values will be used when initializing the SDK in your game.", MessageType.Info);

            EditorGUILayout.Space(15);

            // Server URL
            EditorGUILayout.LabelField("Server URL", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The URL of your Bolt server (e.g., https://api.bolt.com)", MessageType.None);
            _serverUrl = EditorGUILayout.TextField("Server URL", _serverUrl);

            EditorGUILayout.Space(10);

            // App Name
            EditorGUILayout.LabelField("App Name", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("The name of your application as registered with Bolt", MessageType.None);
            _appName = EditorGUILayout.TextField("App Name", _appName);

            EditorGUILayout.Space(15);

            // Validation
            bool isValid = IsValidConfiguration();
            if (!isValid)
            {
                EditorGUILayout.HelpBox("Please fill in all required fields with valid values.", MessageType.Warning);
            }

            EditorGUILayout.Space(10);

            // Buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
            {
                if (isValid)
                {
                    SaveSettings();
                    EditorUtility.DisplayDialog("Bolt SDK", "Settings saved successfully!", "OK");
                }
                else
                {
                    EditorUtility.DisplayDialog("Bolt SDK", "Please fill in all required fields with valid values.", "OK");
                }
            }

            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("Bolt SDK", "Are you sure you want to reset all settings to defaults?", "Yes", "No"))
                {
                    _serverUrl = "";
                    _appName = "";
                    SaveSettings();
                }
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);

            // Test Configuration
            if (GUILayout.Button("Test Configuration", GUILayout.Height(30)))
            {
                TestConfiguration();
            }

            EditorGUILayout.EndScrollView();
        }

        private bool IsValidConfiguration()
        {
            return !string.IsNullOrEmpty(_serverUrl) &&
                   !string.IsNullOrEmpty(_appName) &&
                   System.Uri.IsWellFormedUriString(_serverUrl, System.UriKind.Absolute);
        }

        private void TestConfiguration()
        {
            if (!IsValidConfiguration())
            {
                EditorUtility.DisplayDialog("Bolt SDK", "Please configure valid settings before testing.", "OK");
                return;
            }

            var config = new BoltSDKConfig(_serverUrl, _appName);
            bool isValid = config.IsValid();

            if (isValid)
            {
                EditorUtility.DisplayDialog("Bolt SDK", "Configuration is valid! You can now use the SDK in your game.", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("Bolt SDK", "Configuration is invalid. Please check your settings.", "OK");
            }
        }
    }
}