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
        [SerializeField] private float closeButtonSizePercent = 0.04f; // 4% of screen size
        [SerializeField] private Vector2 closeButtonOffset = new Vector2(20, 20);

        private IBoltWebView webView;
        private GameObject modalBackground;
        private GameObject modalContainer;
        private GameObject closeButton;
        private Canvas modalCanvas;

        // Modal state
        private bool isModalVisible = false;
        private Vector2 modalSize;
        private string currentUrl;

        // Screen resize tracking
        private Vector2 previousScreenSize;

        // Events
        public event Action<string> OnPaymentComplete;
        public event Action<string> OnPaymentError;
        public event Action OnWebViewClosed;

        private void Awake()
        {
            InitializeWebView();
            previousScreenSize = new Vector2(Screen.width, Screen.height);
        }

        private IEnumerator MonitorScreenResize()
        {
            while (isModalVisible)
            {
                yield return new WaitForSeconds(0.1f); // Check every 100ms instead of every frame
                Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
                if (currentScreenSize != previousScreenSize)
                {
                    HandleScreenResize(currentScreenSize);
                    previousScreenSize = currentScreenSize;
                }
            }
        }

        private void HandleScreenResize(Vector2 newScreenSize)
        {
            if (isModalVisible && modalCanvas != null)
            {
                Debug.Log($"Screen resized from {previousScreenSize} to {newScreenSize}. Recalculating modal layout...");

                // Validate that all UI elements still exist before updating
                if (modalBackground == null || modalContainer == null || closeButton == null)
                {
                    Debug.LogWarning("Modal UI elements have been destroyed during resize, forcing cleanup");
                    ForceCleanup();
                    return;
                }

                CalculateModalLayout();
                UpdateModalContainer();
                UpdateCloseButton();
                UpdateWebViewSizeAndPosition();
                webView.LoadPaymentUrl(this.currentUrl);
            }
        }

        private void UpdateModalContainer()
        {
            RectTransform containerRect = modalContainer?.GetComponent<RectTransform>();
            if (containerRect != null)
            {
                Vector2 scaledSize = GetScaledSize(modalSize);
                containerRect.sizeDelta = scaledSize;
                containerRect.anchoredPosition = Vector2.zero; // Centered since anchors are at center
            }
        }

        private void UpdateCloseButton()
        {
            RectTransform closeRect = closeButton?.GetComponent<RectTransform>();
            if (closeRect != null)
            {
                float buttonSize = GetCloseButtonSize();
                float scaledButtonSize = GetScaledValue(buttonSize);
                closeRect.sizeDelta = new Vector2(scaledButtonSize, scaledButtonSize);

                // Position at top-right corner of modal (right edge, top with margin)
                Vector2 scaledSize = GetScaledSize(modalSize);
                Vector2 scaledOffset = GetScaledSize(closeButtonOffset);
                Vector2 closePos = new Vector2(scaledSize.x / 2 - scaledOffset.x, scaledSize.y / 2 + scaledOffset.y + buttonSize / 2);
                closeRect.anchoredPosition = closePos;
            }
        }

        private void UpdateWebViewSizeAndPosition()
        {
            if (webView != null)
            {
                int screenWidth = Screen.width;
                int screenHeight = Screen.height;
                int webViewX = (screenWidth - (int)modalSize.x) / 2;
                int webViewY = (screenHeight - (int)modalSize.y) / 2;

                webView.SetSize((int)modalSize.x, (int)modalSize.y);
                webView.SetPosition(webViewX, webViewY);
            }
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
            if (paymentUrl == null || paymentUrl == "")
            {
                Debug.LogError("Payment URL is null or empty");
                return;
            }

            this.currentUrl = paymentUrl;

            if (webView == null)
            {
                Debug.Log("WebView not initialized. Attempting to initialize...");
                InitializeWebView();
                if (webView == null)
                {
                    Debug.LogError("Failed to initialize WebView");
                    return;
                }
            }

            if (isModalVisible)
            {
                Debug.LogWarning("WebView is already open. Closing existing webview before opening new one.");
                StartCoroutine(CloseAndReopenWebView(paymentUrl));
            }
            else
            {
                StartCoroutine(OpenModalWebView(paymentUrl));
            }
        }

        private IEnumerator OpenModalWebView(string url)
        {
            CalculateModalLayout();

            CreateModalUI();

            yield return ShowModalBackground();

            InitializeWebViewForModal();
            webView.LoadPaymentUrl(url);
            webView.Show();

            isModalVisible = true;
            StartCoroutine(MonitorScreenResize());
        }

        private void CalculateModalLayout()
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            bool isLandscape = screenWidth > screenHeight;
            float aspectRatio;

            if (isLandscape && screenHeight < 1400)
            {
                aspectRatio = 16f / 9f;
            }
            else
            {
                aspectRatio = 9f / 16f;
            }

            int maxWidth = Mathf.RoundToInt(screenWidth * 0.95f);
            int maxHeight = screenHeight - 120 - (int)(GetCloseButtonSize() + closeButtonOffset.y);

            int modalWidth, modalHeight;

            if (aspectRatio > 1f) // 16:9 (landscape)
            {
                // Start with maximum width and calculate height
                modalWidth = maxWidth;
                modalHeight = Mathf.RoundToInt(modalWidth / aspectRatio);

                // If height exceeds max, scale down
                if (modalHeight > maxHeight)
                {
                    modalHeight = maxHeight;
                    modalWidth = Mathf.RoundToInt(modalHeight * aspectRatio);
                }
            }
            else // 9:16 (vertical)
            {
                // Start with maximum height and calculate width
                modalHeight = maxHeight;
                modalWidth = Mathf.RoundToInt(modalHeight * aspectRatio);

                // If width exceeds max, scale down
                if (modalWidth > maxWidth)
                {
                    modalWidth = maxWidth;
                    modalHeight = Mathf.RoundToInt(modalWidth / aspectRatio);
                }
            }

            modalSize = new Vector2(modalWidth, modalHeight);
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

            CreateModalBackground(modalRoot);
            CreateModalContainer(modalRoot);
            CreateCloseButton(modalRoot);

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

        private Vector2 GetScaledSize(Vector2 originalSize)
        {
            if (modalCanvas != null)
            {
                CanvasScaler scaler = modalCanvas.GetComponent<CanvasScaler>();
                if (scaler != null && scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize)
                {
                    float scaleFactor = Screen.width / scaler.referenceResolution.x;
                    return originalSize / scaleFactor;
                }
            }
            return originalSize;
        }

        private float GetScaledValue(float originalValue)
        {
            Vector2 scaledVector = GetScaledSize(new Vector2(originalValue, originalValue));
            return scaledVector.x;
        }

        private float GetCloseButtonSize()
        {
            // Calculate button size as a percentage of screen height
            float screenMax = Mathf.Max(Screen.width, Screen.height);
            float buttonSize = screenMax * closeButtonSizePercent;

            // Apply minimum and maximum bounds for usability
            float minSize = 30f;
            float maxSize = 200f;
            return Mathf.Clamp(buttonSize, minSize, maxSize);
        }

        private void CreateModalContainer(GameObject parent)
        {
            modalContainer = new GameObject("ModalContainer");
            modalContainer.transform.SetParent(parent.transform);

            Image containerImage = modalContainer.AddComponent<Image>();
            containerImage.color = Color.white;

            RectTransform containerRect = modalContainer.GetComponent<RectTransform>();

            containerRect.anchorMin = new Vector2(0.5f, 0.5f);
            containerRect.anchorMax = new Vector2(0.5f, 0.5f);

            Vector2 scaledSize = GetScaledSize(modalSize);
            containerRect.sizeDelta = scaledSize;
            containerRect.anchoredPosition = Vector2.zero;

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
                closeImage.sprite = CreateCircleSprite();
            }
            closeImage.color = closeButtonColor;
            Button closeBtn = closeButton.AddComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                try
                {
                    if (!isModalVisible)
                    {
                        Debug.Log("WebView is already closed or closing");
                        return;
                    }

                    if (modalCanvas == null || modalBackground == null || modalContainer == null || closeButton == null)
                    {
                        Debug.LogWarning("Modal UI elements have been destroyed, forcing cleanup");
                        ForceCleanup();
                        return;
                    }

                    CloseWebView();
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error in close button click handler: {e.Message}");
                    ForceCleanup();
                }
            });

            float buttonSize = GetCloseButtonSize();
            float scaledButtonSize = GetScaledValue(buttonSize);
            RectTransform closeRect = closeButton.GetComponent<RectTransform>();
            if (closeRect != null)
            {
                closeRect.sizeDelta = new Vector2(scaledButtonSize, scaledButtonSize);

                // Position at top-right corner of modal (right edge, top with margin)
                Vector2 scaledSize = GetScaledSize(modalSize);
                Vector2 scaledOffset = GetScaledSize(closeButtonOffset);
                Vector2 closePos = new Vector2(scaledSize.x / 2 - scaledOffset.x, scaledSize.y / 2 + scaledOffset.y + scaledButtonSize / 2);
                closeRect.anchoredPosition = closePos;
            }
            closeButton.SetActive(false);

            // Add "X" text to the close button
            GameObject textGO = new GameObject("CloseText");
            textGO.transform.SetParent(closeButton.transform);

            Text closeText = textGO.AddComponent<Text>();
            closeText.text = "X";
            closeText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            closeText.fontSize = Mathf.RoundToInt(scaledButtonSize * 0.6f);
            closeText.fontStyle = FontStyle.Bold;
            closeText.color = Color.white;
            closeText.alignment = TextAnchor.MiddleCenter;
            closeText.resizeTextForBestFit = true;
            closeText.resizeTextMaxSize = Mathf.RoundToInt(buttonSize * 0.8f);
            closeText.transform.localScale = new Vector3(1, 0.8f, 1); // shorter Y scale to make more square looking X

            RectTransform textRect = closeText.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;
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

            modalBackground.SetActive(true);
            modalContainer.SetActive(true);
            closeButton.SetActive(true);

            yield return StartCoroutine(AnimateCanvasGroups(0f, 1f));
        }

        private IEnumerator AnimateCanvasGroups(float startAlpha, float endAlpha)
        {
            CanvasGroup bgGroup = GetOrAddCanvasGroup(modalBackground);
            CanvasGroup containerGroup = GetOrAddCanvasGroup(modalContainer);
            CanvasGroup closeGroup = GetOrAddCanvasGroup(closeButton);

            if (bgGroup == null || containerGroup == null || closeGroup == null)
            {
                Debug.LogError("Failed to get CanvasGroup components for animation");
                yield break;
            }

            bgGroup.alpha = startAlpha;
            containerGroup.alpha = startAlpha;
            closeGroup.alpha = startAlpha;

            float elapsed = 0f;
            while (elapsed < modalAnimationDuration)
            {
                elapsed += Time.deltaTime;
                float progress = elapsed / modalAnimationDuration;
                float currentAlpha = Mathf.Lerp(startAlpha, endAlpha, progress);

                try
                {
                    bgGroup.alpha = currentAlpha;
                    containerGroup.alpha = currentAlpha;
                    closeGroup.alpha = currentAlpha;
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error during CanvasGroup animation: {e.Message}");
                    break;
                }

                yield return null;
            }

            try
            {
                bgGroup.alpha = endAlpha;
                containerGroup.alpha = endAlpha;
                closeGroup.alpha = endAlpha;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error setting final alpha values: {e.Message}");
            }
        }

        private CanvasGroup GetOrAddCanvasGroup(GameObject obj)
        {
            if (obj == null) return null;

            CanvasGroup group = obj.GetComponent<CanvasGroup>();
            if (group == null)
                group = obj.AddComponent<CanvasGroup>();

            return group;
        }

        private void InitializeWebViewForModal()
        {
            if (webView == null)
            {
                Debug.LogError("WebView not initialized. Cannot set size and position.");
                return;
            }

            UpdateWebViewSizeAndPosition();
        }

        public void CloseWebView()
        {
            if (!isModalVisible)
            {
                Debug.Log("WebView is already closed");
                return;
            }

            if (modalCanvas == null)
            {
                Debug.LogWarning("Modal canvas is null, forcing cleanup");
                ForceCleanup();
                return;
            }

            StartCoroutine(CloseModalWebView());
        }

        private IEnumerator CloseModalWebView()
        {
            StopCoroutine(MonitorScreenResize());

            if (webView != null)
            {
                try
                {
                    webView.Hide();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error hiding WebView: {e.Message}");
                }
            }

            if (modalBackground != null && modalContainer != null && closeButton != null && modalCanvas != null)
            {
                yield return StartCoroutine(AnimateCanvasGroups(1f, 0f));

                try
                {
                    if (modalBackground != null) modalBackground.SetActive(false);
                    if (modalContainer != null) modalContainer.SetActive(false);
                    if (closeButton != null) closeButton.SetActive(false);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error deactivating UI elements: {e.Message}");
                }
            }

            isModalVisible = false;

            CleanupUIElements();
            OnWebViewClosed?.Invoke();
        }

        private IEnumerator CloseAndReopenWebView(string paymentUrl)
        {
            if (isModalVisible)
            {
                yield return StartCoroutine(CloseModalWebView());
                // Wait a frame to ensure cleanup is complete
                yield return null;
            }

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

        private void ForceCleanup()
        {
            try
            {
                Debug.LogWarning("Forcing cleanup of modal UI elements due to destroyed references.");

                // Stop screen resize monitoring
                StopCoroutine(MonitorScreenResize());

                isModalVisible = false;
                CleanupUIElements();
                OnWebViewClosed?.Invoke();
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Error during cleanup: {e.Message}");
            }
        }

        private void CleanupUIElements()
        {
            // Clean up modal UI - destroy the entire canvas hierarchy
            if (modalCanvas != null)
            {
                try
                {
                    DestroyImmediate(modalCanvas.gameObject);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error destroying modal canvas: {e.Message}");
                }
                finally
                {
                    modalCanvas = null;
                    modalBackground = null;
                    modalContainer = null;
                    closeButton = null;
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();

            if (webView != null)
            {
                try
                {
                    webView.Hide();
                    webView.Dispose();
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error disposing WebView: {e.Message}");
                }
                finally
                {
                    webView = null;
                }
            }

            CleanupUIElements();
        }

        private void OnApplicationQuit()
        {
            if (webView != null)
            {
                try
                {
                    if (webView is UnityWebView unityWebView)
                    {
                        unityWebView.ForceDestroy();
                    }
                    else
                    {
                        webView.Dispose();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error force destroying WebView: {e.Message}");
                }
                finally
                {
                    webView = null;
                }
            }
        }
    }
}
