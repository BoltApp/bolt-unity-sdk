using UnityEngine;
using BoltApp;

namespace BoltApp.Samples // TODO: replace with your own namespace
{
    public class BoltClient : MonoBehaviour
    {
        private static BoltClient _instance;
        private BoltSDK _boltSDK;
        private IAdWebViewService _adWebViewService;

        // Configuration - set these to your values
        private const string GAME_ID = "com.myapp.test";
        private const string PUBLISHABLE_KEY = "example.publishable.key";

        public static BoltClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BoltClient");
                    _instance = go.AddComponent<BoltClient>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        public BoltSDK SDK
        {
            get
            {
                if (_boltSDK == null)
                {
                    var environment = BoltConfig.Environment.Sandbox; // Set to your environment
                    var boltConfig = new BoltConfig(GAME_ID, PUBLISHABLE_KEY, environment);

#if UNIWEBVIEW
                    _adWebViewService = new UniWebViewAdService();
                    _boltSDK = new BoltSDK(boltConfig, _adWebViewService);
#else
                    _boltSDK = new BoltSDK(boltConfig);
#endif

                    // Subscribe to ad events
                    _boltSDK.onAdOpened += OnAdOpened;
                    _boltSDK.onAdCompleted += OnAdCompleted;
                }
                return _boltSDK;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Call once at game start to preload first ad.
        /// Subsequent ads are preloaded automatically when the current ad completes.
        /// </summary>
        public void InitializeAds()
        {
            var sdk = SDK;
            if (sdk == null) return;

            var session = sdk.PreloadAd();
            if (session == null)
                Debug.LogError("[BoltClient] PreloadAd failed.");
        }

        /// <summary>
        /// Shows the preloaded ad. Returns true if the ad was shown, false if none available or failed.
        /// </summary>
        public bool ShowAd()
        {
            // The id associated with the button clicked to show the ad
            var buttonID = "homePageButton";

            // Any metadata you want to associate with the ad object
            var myInternalUserID = "example_user_id";
            var myInternalTrackingID = "example_tracking_id";
            var metadata = AdMetaData.New().Add("user_id", myInternalTrackingID).Add("tracking_id", myInternalTrackingID);
            var result = SDK.ShowAd(buttonID, AdPlacement.MainMenu, metadata);
            return result != null && result.Status != AdStatus.Failed;
        }

        private void OnAdOpened()
        {
            Debug.Log($"[BoltClient] Ad opened");
        }

        private void OnAdCompleted()
        {
            Debug.Log($"[BoltClient] Ad completed");
        }

        void OnApplicationPause(bool pauseStatus)
        {
#if UNIWEBVIEW
            if (pauseStatus && _adWebViewService != null)
            {
                _adWebViewService.Cleanup();
            }
#endif
        }

        private void OnDestroy()
        {
            if (_boltSDK != null)
            {
                _boltSDK.onAdOpened -= OnAdOpened;
                _boltSDK.onAdCompleted -= OnAdCompleted;
            }
        }
    }
}