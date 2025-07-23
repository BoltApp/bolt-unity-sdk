using UnityEngine;
using UnityEditor;
using BoltSDK;

namespace BoltSDK.Editor
{
    /// <summary>
    /// Simple editor window for Bolt SDK Configuration
    /// </summary>
    public class BoltConfigWindow : EditorWindow
    {
        private BoltConfig config;
        private const string CONFIG_PATH = "Assets/BoltConfig.asset";

        [MenuItem("Tools/Bolt SDK/Configuration")]
        public static void ShowWindow()
        {
            var window = GetWindow<BoltConfigWindow>("Bolt SDK Configuration");
            window.minSize = new Vector2(400, 250);
            window.Show();
        }

        private void OnEnable()
        {
            LoadOrCreateConfig();
        }

        private void LoadOrCreateConfig()
        {
            // Try to load existing configuration
            config = AssetDatabase.LoadAssetAtPath<BoltConfig>(CONFIG_PATH);

            if (config == null)
            {
                // Create new configuration
                config = ScriptableObject.CreateInstance<BoltConfig>();
                AssetDatabase.CreateAsset(config, CONFIG_PATH);
                AssetDatabase.SaveAssets();
                Debug.Log("Created new Bolt SDK Configuration");
            }
        }

        private void OnGUI()
        {
            if (config == null)
            {
                LoadOrCreateConfig();
                return;
            }

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Bolt SDK Configuration", EditorStyles.boldLabel);
            EditorGUILayout.Space(10);

            // Configuration fields with automatic saving
            EditorGUI.BeginChangeCheck();

            config.gameId = EditorGUILayout.TextField("Game ID", config.gameId);
            EditorGUILayout.HelpBox("Your unique game identifier from Bolt", MessageType.Info);

            EditorGUILayout.Space(5);
            config.deepLinkAppName = EditorGUILayout.TextField("Deep Link App Name", config.deepLinkAppName);
            EditorGUILayout.HelpBox("Your deep link app name for handling callbacks", MessageType.Info);

            EditorGUILayout.Space(10);
            config.environment = (BoltConfig.Environment)EditorGUILayout.EnumPopup("Environment", config.environment);
            EditorGUILayout.HelpBox("Select the environment for your game", MessageType.Info);

            // Auto-save when any field changes
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(config);
                AssetDatabase.SaveAssets();
            }

            EditorGUILayout.Space(20);

            // Validation
            bool isValid = config.IsValid();
            if (isValid)
            {
                EditorGUILayout.HelpBox("✓ Configuration is valid", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("✗ Please fill in all required fields", MessageType.Error);
            }

            EditorGUILayout.Space(10);

            // Action buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Reset Configuration",
                    "Reset all fields to default values?", "Reset", "Cancel"))
                {
                    config.gameId = "";
                    config.deepLinkAppName = "";
                    config.environment = BoltConfig.Environment.Development;
                    EditorUtility.SetDirty(config);
                    AssetDatabase.SaveAssets();
                }
            }

            if (GUILayout.Button("Open in Inspector", GUILayout.Height(25)))
            {
                Selection.activeObject = config;
                EditorUtility.FocusProjectWindow();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Changes are automatically saved. You can also edit this configuration in the Inspector.", MessageType.Info);
        }
    }
}