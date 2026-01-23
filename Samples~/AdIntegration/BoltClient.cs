using System.Threading.Tasks;
using UnityEngine;
using BoltApp;

namespace BoltApp.Samples
{
    public class BoltClient : MonoBehaviour
    {
        private static BoltClient _instance;
        private BoltSDK _boltSDK;
        private IAdWebViewService _adWebViewService;
        private AdSession _preloadedAdSession;

        // Configuration - set these to your values
        private const string GAME_ID = "com.myapp.test";
        private const string PUBLISHABLE_KEY = "example.publishable.key";
        private const string AD_LINK = "https://play.staging-bolt.com/";

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
                    var environment = BoltConfig.Environment.Development; // Set to your environment
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
        /// Preloads the ad (call once at game start)
        /// </summary>
        public void PreloadAd(string adLink, AdType adType = AdType.Timed)
        {
            var sdk = SDK;
            if (sdk == null) return;

            _preloadedAdSession = sdk.PreloadAd(adLink, adType);
        }

        /// <summary>
        /// Shows the preloaded ad (can be called many times from buttons)
        /// Returns true if ad show was initiated successfully, false otherwise.
        /// Note: Ad completion is handled via OnAdCompleted callback.
        /// </summary>
        public async Task<bool> ShowAd()
        {
            if (_preloadedAdSession == null || !_preloadedAdSession.IsValid())
            {
                Debug.LogError("[BoltClient] Ad not preloaded. Call PreloadAd() first.");
                return false;
            }

            await _preloadedAdSession.Show();
            return _preloadedAdSession.Status == AdStatus.Showing;
        }

        private void OnAdOpened(string adLink)
        {
            Debug.Log($"[BoltClient] Ad opened: {adLink}");
        }

        private void OnAdCompleted(string adLink)
        {
            Debug.Log($"[BoltClient] Ad completed: {adLink}");
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
