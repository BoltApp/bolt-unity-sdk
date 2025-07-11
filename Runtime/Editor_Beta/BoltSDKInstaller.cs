using UnityEngine;
using UnityEditor;
using System.IO;
using System.Net;
using System.Collections;
using System.Collections.Generic;

namespace BoltSDK.Editor
{
    /**
     * Note - this is in beta and is not yet ready for production use.
     * This is an editor window that helps install dependencies for the Bolt SDK.
     */
    public class BoltSDKInstaller : EditorWindow
    {
        private Vector2 scrollPosition;
        private bool isInstalling = false;
        private string installStatus = "";
        private float installProgress = 0f;

        // Checkbox state variables
        private bool installWindows = true;
        private bool installMobile = true;
        private bool installWebGL = true;

        // Download URLs for different platforms
        private readonly Dictionary<string, string> downloadUrls = new Dictionary<string, string>
        {
            { "Windows", "https://github.com/bolt/bolt-webview-binaries/releases/latest/download/bolt-webview-windows.zip" },
            { "UnityWebView", "https://github.com/gree/unity-webview/raw/refs/heads/master/dist/unity-webview.zip" }
        };

        // Installation paths
        private readonly string pluginsPath = "Assets/bolt-unity-sdk-main/Plugins";
        private readonly string windowsPluginPath = "Assets/bolt-unity-sdk-main/Plugins/Windows/x86_64";
        private readonly string unityWebViewPath = "Assets/bolt-unity-sdk-main/Plugins/UnityWebView";

        [MenuItem("Bolt SDK/Install Dependencies")]
        public static void ShowWindow()
        {
            GetWindow<BoltSDKInstaller>("Bolt SDK Installer");
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("Install Dependencies", EditorStyles.boldLabel);

            EditorGUILayout.HelpBox("This installer will download and configure WebView dependencies for your target platforms.", MessageType.Info);
            GUILayout.Space(10);

            // Platform selection
            GUILayout.Label("Target Platforms:", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Windows (CEF)", GUILayout.Width(180));
            installWindows = GUILayout.Toggle(installWindows, "", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("iOS/Android/Mac (Webview)", GUILayout.Width(180));
            installMobile = GUILayout.Toggle(installMobile, "", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("WebGL (JavaScript)", GUILayout.Width(180));
            installWebGL = GUILayout.Toggle(installWebGL, "", GUILayout.Width(20));
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (isInstalling)
            {
                GUILayout.Label("Progress:", EditorStyles.boldLabel);
                EditorGUI.ProgressBar(GUILayoutUtility.GetRect(0, 20), installProgress, $"{installStatus}... {installProgress:P0}");
            }

            GUILayout.Space(10);

            // Install button
            GUI.enabled = !isInstalling;
            if (GUILayout.Button("Install WebView Dependencies", GUILayout.Height(30)))
            {
                StartInstallation(installWindows, installMobile, installWebGL);
            }

            GUILayout.Space(10);

            // Installation status
            if (!string.IsNullOrEmpty(installStatus) && !isInstalling)
            {
                GUILayout.Label("Status:", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(installStatus, MessageType.Info);
            }

            if (GUILayout.Button("Test WebView Installation", GUILayout.Height(30)))
            {
                TestWebViewInstallation();
            }

            // Re-enable UI
            GUI.enabled = true;
            GUILayout.Space(10);

            // Information
            GUILayout.Label("Information:", EditorStyles.boldLabel);
            GUILayout.Label("• Windows: Downloads CEF binaries (~100MB)", EditorStyles.miniLabel);
            GUILayout.Label("• Mobile/Mac: Downloads Unity WebView package", EditorStyles.miniLabel);
            GUILayout.Label("• WebGL: Uses browser's native window functionality", EditorStyles.miniLabel);

            EditorGUILayout.EndScrollView();
        }

        private void StartInstallation(bool installWindows, bool installMobile, bool installWebGL)
        {
            isInstalling = true;
            installProgress = 0f;
            installStatus = "Starting installation...";

            EditorCoroutine.Start(InstallDependencies(installWindows, installMobile, installWebGL));

            // Force the window to repaint during installation
            EditorApplication.update += RepaintWindow;
        }

        private void RepaintWindow()
        {
            if (isInstalling)
            {
                Repaint();
            }
            else
            {
                EditorApplication.update -= RepaintWindow;
            }
        }

        private IEnumerator InstallDependencies(bool installWindows, bool installMobile, bool installWebGL)
        {
            CreateDirectories();
            installProgress = 0.1f;
            installStatus = "Created directories...";
            yield return null;

            // Install Windows CEF
            if (installWindows)
            {
                installStatus = "Installing Windows CEF binaries...";
                yield return InstallWindowsCEF();
                installProgress = 0.4f;
            }

            // Install Unity WebView
            if (installMobile)
            {
                installStatus = "Installing Unity WebView...";
                yield return InstallUnityWebView();
                installProgress = 0.7f;
            }

            try
            {

                // Configure WebGL
                if (installWebGL)
                {
                    try
                    {
                        installStatus = "Configuring WebGL...";
                        ConfigureWebGL();
                        installProgress = 0.9f;
                    }
                    catch (System.Exception e)
                    {
                        installStatus = $"WebGL configuration failed: {e.Message}";
                        Debug.LogError($"Bolt SDK Installer: WebGL configuration failed: {e}");
                    }
                }

                // Final configuration
                try
                {
                    installProgress = 1f;
                    AssetDatabase.Refresh();
                }
                catch (System.Exception e)
                {
                    installStatus = $"Final configuration failed: {e.Message}";
                    Debug.LogError($"Bolt SDK Installer: Final configuration failed: {e}");
                }
            }
            catch (System.Exception e)
            {
                installStatus = $"Installation failed: {e.Message}";
                Debug.LogError($"Bolt SDK Installer: Installation failed: {e}");
            }
            finally
            {
                Debug.Log("Bolt SDK Installer: Installation process finished");
                isInstalling = false;
            }
        }

        private void CreateDirectories()
        {
            Directory.CreateDirectory(pluginsPath);
            Directory.CreateDirectory(windowsPluginPath);
            Directory.CreateDirectory(unityWebViewPath);
        }

        private IEnumerator InstallWindowsCEF()
        {
            string downloadUrl = downloadUrls["Windows"];
            string tempPath = Path.Combine(Path.GetTempPath(), "bolt-webview-windows.zip");
            string extractPath = windowsPluginPath;

            Debug.Log($"Bolt SDK Installer: Downloading Windows CEF from: {downloadUrl}");
            Debug.Log($"Bolt SDK Installer: Temporary file path: {tempPath}");
            Debug.Log($"Bolt SDK Installer: Extract path: {extractPath}");

            // Download CEF binaries
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, e) =>
                {
                    installProgress = 0.1f + (e.ProgressPercentage * 0.2f);
                    if (e.ProgressPercentage % 10 == 0) // Log every 10%
                    {
                        Debug.Log($"Bolt SDK Installer: Windows CEF download progress: {e.ProgressPercentage}%");
                    }
                };

                client.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                    {
                        Debug.LogError($"Bolt SDK Installer: Failed to download Windows CEF: {e.Error.Message}");
                    }
                };

                client.DownloadFileAsync(new System.Uri(downloadUrl), tempPath);

                // Wait for download to complete
                while (client.IsBusy)
                {
                    yield return null;
                }
            }

            try
            {
                // Extract files
                if (File.Exists(tempPath))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(tempPath, extractPath);
                    File.Delete(tempPath);
                }
                else
                {
                    Debug.LogError("Bolt SDK Installer: Downloaded file not found for extraction");
                }
            }
            catch (System.Exception e)
            {
                installStatus = $"Windows CEF installation failed: {e.Message}";
                Debug.LogError($"Bolt SDK Installer: Windows CEF installation failed: {e}");
                throw; // Re-throw to be caught by the main method
            }
        }

        private IEnumerator InstallUnityWebView()
        {
            string downloadUrl = downloadUrls["UnityWebView"];
            string tempPath = Path.Combine(Path.GetTempPath(), "unity-webview.unitypackage");

            // Download Unity WebView package
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += (sender, e) =>
                {
                    installProgress = 0.4f + (e.ProgressPercentage * 0.2f);
                };

                client.DownloadFileAsync(new System.Uri(downloadUrl), tempPath);

                // Wait for download to complete
                while (client.IsBusy)
                {
                    yield return null;
                }
            }

            try
            {
                // Import Unity package
                if (File.Exists(tempPath))
                {
                    AssetDatabase.ImportPackage(tempPath, false);
                    File.Delete(tempPath);
                }
                else
                {
                    Debug.LogError("Bolt SDK Installer: Downloaded Unity WebView package not found for import");
                }
            }
            catch (System.Exception e)
            {
                installStatus = $"Unity WebView installation failed: {e.Message}";
                Debug.LogError($"Bolt SDK Installer: Unity WebView installation failed: {e}");
                throw; // Re-throw to be caught by the main method
            }
        }

        private void ConfigureWebGL()
        {
            // Create WebGL JavaScript plugin
            string webglPluginPath = "Assets/bolt-unity-sdk-main/Plugins/WebGL";
            Directory.CreateDirectory(webglPluginPath);

            string jsPluginContent = @"
mergeInto(LibraryManager.library, {
    OpenWebViewWindow: function(url, windowName, width, height) {
        var urlStr = UTF8ToString(url);
        var nameStr = UTF8ToString(windowName);
        var popup = window.open(urlStr, nameStr, 'width=' + width + ',height=' + height);
        if (popup) {
            popup.focus();
        }
    },
    
    CloseWebViewWindow: function() {
        // Close any open popup windows
        if (window.boltPaymentWindow) {
            window.boltPaymentWindow.close();
        }
    },
    
    IsWebViewWindowOpen: function() {
        return window.boltPaymentWindow && !window.boltPaymentWindow.closed;
    },
    
    SetWebViewCallback: function(callbackName) {
        window.HandleWebViewCallback = function(message) {
            // Handle payment completion
            if (message.includes('payment_complete')) {
                SendMessage('WebGLWebView', 'HandlePaymentComplete', message);
            } else if (message.includes('error')) {
                SendMessage('WebGLWebView', 'HandlePaymentError', message);
            }
        };
    }
});
";

            File.WriteAllText(Path.Combine(webglPluginPath, "BoltWebView.jslib"), jsPluginContent);
        }

        private void TestWebViewInstallation()
        {
            // Check if required files exist
            bool windowsOK = Directory.Exists(windowsPluginPath) &&
                           File.Exists(Path.Combine(windowsPluginPath, "BoltWebViewPlugin.dll"));

            bool unityWebViewOK = Directory.Exists(unityWebViewPath);

            bool webglOK = File.Exists("Assets/bolt-unity-sdk-main/Plugins/WebGL/BoltWebView.jslib");

            string status = $"Installation Test Results:\n" +
                          $"Windows CEF: {(windowsOK ? "OK" : "Missing")}\n" +
                          $"Unity WebView: {(unityWebViewOK ? "OK" : "Missing")}\n" +
                          $"WebGL Plugin: {(webglOK ? "OK" : "Missing")}";

            installStatus = status;
            Debug.Log(status);
        }
    }

    // Helper class for running coroutines in Editor
    public static class EditorCoroutine
    {
        private static List<IEnumerator> activeCoroutines = new List<IEnumerator>();

        public static void Start(IEnumerator routine)
        {
            activeCoroutines.Add(routine);
            EditorApplication.update += UpdateCoroutines;
        }

        private static void UpdateCoroutines()
        {
            for (int i = activeCoroutines.Count - 1; i >= 0; i--)
            {
                if (!activeCoroutines[i].MoveNext())
                {
                    activeCoroutines.RemoveAt(i);
                }
            }

            if (activeCoroutines.Count == 0)
            {
                EditorApplication.update -= UpdateCoroutines;
            }
        }
    }
}