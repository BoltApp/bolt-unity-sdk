using UnityEngine;
using UnityEngine.UI;

namespace BoltSDK.Samples
{
    /// <summary>
    /// Simple setup script that creates a basic UI for the basic scene
    /// </summary>
    public class SimpleSceneSetup : MonoBehaviour
    {
        [Header("Setup Options")]
        [SerializeField] private bool autoSetupOnStart = true;

        [Header("UI Settings")]
        [SerializeField] private Vector2 buttonSize = new Vector2(200, 50);
        [SerializeField] private Color buttonColor = Color.blue;
        [SerializeField] private Color textColor = Color.white;

        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupBasicUI();
            }
        }

        /// <summary>
        /// Set up the basic UI with buttons
        /// </summary>
        [ContextMenu("Setup Basic UI")]
        public void SetupBasicUI()
        {
            // Create or find canvas
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
            }

            // Create UI container
            GameObject containerObj = new GameObject("UIContainer");
            containerObj.transform.SetParent(canvas.transform, false);

            RectTransform containerRect = containerObj.AddComponent<RectTransform>();
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);
            containerRect.sizeDelta = new Vector2(300, 200);
            containerRect.anchoredPosition = Vector2.zero;

            // Create web link button
            CreateButton(containerObj, "OpenWebLinkButton", "Open Web Link", 0, buttonSize, buttonColor, textColor);

            // Create checkout button
            CreateButton(containerObj, "OpenCheckoutButton", "Open Checkout", 1, buttonSize, Color.green, textColor);

            // Create status text
            CreateStatusText(containerObj, "StatusText", "Ready");

            // Create or find BasicScene component
            BasicScene basicScene = FindObjectOfType<BasicScene>();
            if (basicScene == null)
            {
                GameObject sceneObj = new GameObject("BasicScene");
                basicScene = sceneObj.AddComponent<BasicScene>();
            }

            // Assign UI references to BasicScene
            AssignUIReferences(basicScene, containerObj);

            Debug.Log("Basic scene UI setup completed!");
        }

        /// <summary>
        /// Create a button
        /// </summary>
        private void CreateButton(GameObject parent, string name, string text, int index, Vector2 size, Color color, Color textColor)
        {
            GameObject buttonObj = new GameObject(name);
            buttonObj.transform.SetParent(parent.transform, false);

            RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            rectTransform.anchoredPosition = new Vector2(0, -index * (size.y + 10));

            Image image = buttonObj.AddComponent<Image>();
            image.color = color;

            Button button = buttonObj.AddComponent<Button>();

            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(buttonObj.transform, false);

            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            Text textComponent = textObj.AddComponent<Text>();
            textComponent.text = text;
            textComponent.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            textComponent.fontSize = 14;
            textComponent.alignment = TextAnchor.MiddleCenter;
            textComponent.color = textColor;
        }

        /// <summary>
        /// Create status text
        /// </summary>
        private void CreateStatusText(GameObject parent, string name, string initialText)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent.transform, false);

            RectTransform rectTransform = textObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(280, 30);
            rectTransform.anchoredPosition = new Vector2(0, -120);

            Text text = textObj.AddComponent<Text>();
            text.text = initialText;
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 12;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;
        }

        /// <summary>
        /// Assign UI references to the BasicScene component
        /// </summary>
        private void AssignUIReferences(BasicScene basicScene, GameObject containerObj)
        {
            // Use reflection to set the private fields
            var openWebLinkButtonField = typeof(BasicScene).GetField("openWebLinkButton",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var openCheckoutButtonField = typeof(BasicScene).GetField("openCheckoutButton",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var statusTextField = typeof(BasicScene).GetField("statusText",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (openWebLinkButtonField != null)
            {
                Button webButton = containerObj.transform.Find("OpenWebLinkButton")?.GetComponent<Button>();
                openWebLinkButtonField.SetValue(basicScene, webButton);
            }

            if (openCheckoutButtonField != null)
            {
                Button checkoutButton = containerObj.transform.Find("OpenCheckoutButton")?.GetComponent<Button>();
                openCheckoutButtonField.SetValue(basicScene, checkoutButton);
            }

            if (statusTextField != null)
            {
                Text statusText = containerObj.transform.Find("StatusText")?.GetComponent<Text>();
                statusTextField.SetValue(basicScene, statusText);
            }
        }

        /// <summary>
        /// Clean up the scene
        /// </summary>
        [ContextMenu("Cleanup Scene")]
        public void CleanupScene()
        {
            BasicScene basicScene = FindObjectOfType<BasicScene>();
            if (basicScene != null)
            {
                DestroyImmediate(basicScene.gameObject);
            }

            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null && canvas.name == "Canvas")
            {
                DestroyImmediate(canvas.gameObject);
            }

            Debug.Log("Scene cleanup completed!");
        }
    }
}