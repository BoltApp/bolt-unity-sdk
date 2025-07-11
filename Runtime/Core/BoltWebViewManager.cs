using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace BoltSDK
{
    public class BoltWebViewManager : MonoBehaviour
    {
        [Header("Modal Settings")]
        [SerializeField] private bool useModalMode = true;
        [SerializeField] private Color modalBackgroundColor = new Color(0, 0, 0, 0.8f);
        [SerializeField] private float modalAnimationDuration = 0.3f;

        [Header("Close Button")]
        [SerializeField] private Sprite closeButtonSprite;
        [SerializeField] private Color closeButtonColor = new Color(1, 1, 1, 0.2f);
        [SerializeField] private float closeButtonSize = 40f;
        [SerializeField] private Vector2 closeButtonOffset = new Vector2(20, 20);

        private IBoltWebView webView;
        private GameObject modalBackground;
        private GameObject modalContainer;
        private GameObject closeButton;
        private Canvas modalCanvas;

        // Modal state
        private bool isModalVisible = false;
        private Vector2 modalSize;
        private Vector2 modalPosition;

        // Events
        public event Action<string> OnPaymentComplete;
        public event Action<string> OnPaymentError;
        public event Action OnWebViewClosed;

        private void Awake()
        {
            InitializeWebView();
        }

        private void InitializeWebView()
        {
            webView = WebViewFactory.CreateWebView();
            webView.OnPaymentComplete += HandlePaymentComplete;
            webView.OnError += HandlePaymentError;
            webView.OnWebViewClosed += HandleWebViewClosed;
        }

        public void OpenPaymentWebView(string paymentUrl)
        {
            if (webView == null)
            {
                Debug.LogError("WebView not initialized. Attempting to initialize...");
                InitializeWebView();
                if (webView == null)
                {
                    Debug.LogError("Failed to initialize WebView");
                    return;
                }
            }

            // If webview is already visible, close and reopen with new URL
            if (isModalVisible)
            {
                Debug.LogWarning("WebView is already open. Closing existing webview before opening new one.");
                StartCoroutine(CloseAndReopenWebView(paymentUrl));
            }
            else
            {
                // Open new webview directly
                StartCoroutine(OpenModalWebView(paymentUrl));
            }
        }

        private IEnumerator OpenModalWebView(string url)
        {
            // Calculate modal dimensions and position
            CalculateModalLayout();

            // Create modal UI elements
            CreateModalUI();

            // Show modal background with animation
            yield return ShowModalBackground();

            // Initialize and show WebView
            InitializeWebViewForModal();
            webView.LoadPaymentUrl(url);
            webView.Show();

            isModalVisible = true;
        }

        private void CalculateModalLayout()
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;

            // Calculate modal size (9:16 aspect ratio - vertical)
            float aspectRatio = 9f / 16f;
            int maxWidth = screenWidth - 80; // 40px padding on each side
            int maxHeight = screenHeight - 120; // 60px padding top/bottom

            int modalHeight = maxHeight;
            int modalWidth = Mathf.RoundToInt(modalHeight * aspectRatio);

            // If width exceeds max, scale down
            if (modalWidth > maxWidth)
            {
                modalWidth = maxWidth;
                modalHeight = Mathf.RoundToInt(modalWidth / aspectRatio);
            }

            modalSize = new Vector2(modalWidth, modalHeight);
            modalPosition = new Vector2((screenWidth - modalWidth) / 2, (screenHeight - modalHeight) / 2);
        }

        private void CreateModalUI()
        {
            if (modalCanvas != null)
            {
                DestroyImmediate(modalCanvas.gameObject);
                modalCanvas = null;
            }

            GameObject canvasGO = new GameObject("BoltWebViewCanvas");
            modalCanvas = canvasGO.AddComponent<Canvas>();
            modalCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            modalCanvas.sortingOrder = 1000; // Ensure it's on top

            // Add canvas scaler
            CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);

            // Add graphic raycaster for UI interactions
            canvasGO.AddComponent<GraphicRaycaster>();

            // Create a container for all modal elements
            GameObject modalRoot = new GameObject("ModalRoot");
            modalRoot.transform.SetParent(modalCanvas.transform);
            RectTransform modalRootRect = modalRoot.AddComponent<RectTransform>();
            modalRootRect.anchorMin = Vector2.zero;
            modalRootRect.anchorMax = Vector2.one;
            modalRootRect.offsetMin = Vector2.zero;
            modalRootRect.offsetMax = Vector2.zero;

            // Create background overlay
            CreateModalBackground(modalRoot);

            // Create modal container (white rectangle)
            CreateModalContainer(modalRoot);

            // Create close button
            CreateCloseButton(modalRoot);

            // Verify creation was successful
            if (modalBackground == null || modalContainer == null || closeButton == null)
            {
                Debug.LogError("Failed to create modal UI elements");
            }
        }

        private void CreateModalBackground(GameObject parent)
        {
            modalBackground = new GameObject("ModalBackground");
            modalBackground.transform.SetParent(parent.transform);

            Image bgImage = modalBackground.AddComponent<Image>();
            bgImage.color = modalBackgroundColor;

            RectTransform bgRect = modalBackground.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            modalBackground.SetActive(false);
        }

        private void CreateModalContainer(GameObject parent)
        {
            modalContainer = new GameObject("ModalContainer");
            modalContainer.transform.SetParent(parent.transform);

            Image containerImage = modalContainer.AddComponent<Image>();
            containerImage.color = Color.white; // White rectangle

            RectTransform containerRect = modalContainer.GetComponent<RectTransform>();

            // Set anchors to center
            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);

            // Set size and position
            containerRect.sizeDelta = modalSize;
            containerRect.anchoredPosition = Vector2.zero; // Centered since anchors are at center

            modalContainer.SetActive(false);
        }

        private void CreateCloseButton(GameObject parent)
        {
            closeButton = new GameObject("CloseButton");
            closeButton.transform.SetParent(parent.transform);

            Image closeImage = closeButton.AddComponent<Image>();
            if (closeButtonSprite != null)
            {
                closeImage.sprite = closeButtonSprite;
            }
            else
            {
                // Create a simple circle sprite if none provided
                closeImage.sprite = CreateCircleSprite();
            }
            closeImage.color = closeButtonColor;

            Button closeBtn = closeButton.AddComponent<Button>();
            closeBtn.onClick.AddListener(() => CloseWebView());

            RectTransform closeRect = closeButton.GetComponent<RectTransform>();
            closeRect.sizeDelta = new Vector2(closeButtonSize, closeButtonSize);

            // Position close button at top-right of modal container (centered)
            Vector2 closePos = new Vector2(modalSize.x / 2 - closeButtonOffset.x, modalSize.y / 2 - closeButtonOffset.y);
            closeRect.anchoredPosition = closePos;

            closeButton.SetActive(false);
        }

        private Sprite CreateCircleSprite()
        {
            int size = 64;
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            Vector2 center = new Vector2(size / 2f, size / 2f);
            float radius = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * size + x] = distance <= radius ? Color.white : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }

        private IEnumerator ShowModalBackground()
        {
            if (modalBackground == null || modalContainer == null || closeButton == null)
            {
                Debug.LogError("Modal UI elements not created. Cannot show modal background.");
                yield break;
            }

            // Show all elements immediately
            modalBackground.SetActive(true);
            modalContainer.SetActive(true);
            closeButton.SetActive(true);

            // Fade in animation
            CanvasGroup bgGroup = modalBackground.GetComponent<CanvasGroup>();
            if (bgGroup == null)
                bgGroup = modalBackground.AddComponent<CanvasGroup>();

            CanvasGroup containerGroup = modalContainer.GetComponent<CanvasGroup>();
            if (containerGroup == null)
                containerGroup = modalContainer.AddComponent<CanvasGroup>();

            CanvasGroup closeGroup = closeButton.GetComponent<CanvasGroup>();
            if (closeGroup == null)
                closeGroup = closeButton.AddComponent<CanvasGroup>();

            bgGroup.alpha = 0f;
            containerGroup.alpha = 0f;
            closeGroup.alpha = 0f;

            float elapsed = 0f;
            while (elapsed < modalAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / modalAnimationDuration;

                bgGroup.alpha = progress;
                containerGroup.alpha = progress;
                closeGroup.alpha = progress;

                yield return null;
            }

            bgGroup.alpha = 1f;
            containerGroup.alpha = 1f;
            closeGroup.alpha = 1f;
        }

        private void InitializeWebViewForModal()
        {
            if (webView == null)
            {
                Debug.LogError("WebView not initialized. Cannot set size and position.");
                return;
            }

            // Calculate the center position for the WebView
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            int webViewX = (screenWidth - (int)modalSize.x) / 2;
            int webViewY = (screenHeight - (int)modalSize.y) / 2;

            // Set webview size and position to match the modal container exactly
            webView.SetSize((int)modalSize.x, (int)modalSize.y);
            webView.SetPosition(webViewX, webViewY);
        }

        public void CloseWebView()
        {
            if (!isModalVisible) return;

            StartCoroutine(CloseModalWebView());
        }

        private IEnumerator CloseModalWebView()
        {
            // Hide WebView
            if (webView != null)
            {
                webView.Hide();
                webView.Close();
            }

            // Fade out modal background if it exists
            if (modalBackground != null && modalContainer != null && closeButton != null)
            {
                CanvasGroup bgGroup = modalBackground.GetComponent<CanvasGroup>();
                CanvasGroup containerGroup = modalContainer.GetComponent<CanvasGroup>();
                CanvasGroup closeGroup = closeButton.GetComponent<CanvasGroup>();

                float elapsed = 0f;
                while (elapsed < modalAnimationDuration)
                {
                    elapsed += Time.deltaTime;
                    float progress = elapsed / modalAnimationDuration;

                    bgGroup.alpha = 1f - progress;
                    containerGroup.alpha = 1f - progress;
                    closeGroup.alpha = 1f - progress;

                    yield return null;
                }

                // Clean up UI elements
                modalBackground.SetActive(false);
                modalContainer.SetActive(false);
                closeButton.SetActive(false);
            }

            isModalVisible = false;

            // Clean up the entire canvas hierarchy
            if (modalCanvas != null)
            {
                DestroyImmediate(modalCanvas.gameObject);
                modalCanvas = null;
                modalBackground = null;
                modalContainer = null;
                closeButton = null;
            }

            OnWebViewClosed?.Invoke();
        }

        private IEnumerator CloseAndReopenWebView(string paymentUrl)
        {
            // Close the current webview if it's visible
            if (isModalVisible)
            {
                yield return StartCoroutine(CloseModalWebView());
                // Wait a frame to ensure cleanup is complete
                yield return null;
            }

            // Open the new webview
            yield return StartCoroutine(OpenModalWebView(paymentUrl));
        }

        private void HandlePaymentComplete(string result)
        {
            OnPaymentComplete?.Invoke(result);
            CloseWebView();
        }

        private void HandlePaymentError(string error)
        {
            OnPaymentError?.Invoke(error);
            CloseWebView();
        }

        private void HandleWebViewClosed()
        {
            if (isModalVisible)
            {
                CloseWebView();
            }
        }


        private void OnDestroy()
        {
            webView?.Dispose();

            // Clean up modal UI - destroy the entire canvas hierarchy
            if (modalCanvas != null)
            {
                DestroyImmediate(modalCanvas.gameObject);
                modalCanvas = null;
            }

            // Clear references
            modalBackground = null;
            modalContainer = null;
            closeButton = null;
        }
    }
}