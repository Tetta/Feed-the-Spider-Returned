using System;
using KiiCorp.Cloud.Storage;
using KiiCorp.Cloud.Storage;
using KiiCorp.Cloud.Analytics;
using UnityEngine;

namespace KiiCorp.Cloud.Unity
{
    /// <summary>
    /// Mandatory MonoBehaviour for scenes that use KiiCloud.
    /// You need to use this class to initialize Kii SDK instead of <see cref="Kii.Initialize(string, string, Kii.Site)"/>/<see cref="KiiAnalytics.Initialize(string, string, KiiAnalytics.Site, string)"/> method if you want to enjoy a lot of benefits.
    /// </summary>
    /// <remarks>
    /// How to setup.
    /// <list type="number">
    /// <item>
    /// <term>Create an empty GameObject and attach the KiiInitializeBehaviour script component to it.</term>
    /// </item>
    /// <item>
    /// <term>Specify your AppID, AppKey and Site in the KiiInitializeBehaviour script.</term>
    /// </item>
    /// </list>
    /// </remarks>
    public class KiiInitializeBehaviour : MonoBehaviour
    {
        private static string PREFS_KEY_DEVICE_ID = "KiiCorp.Cloud.Unity.KiiInitializeBehaviour.DeviceID";
        private static bool isInitialized;
        private static KiiInitializeBehaviour INSTANCE;

        public static KiiInitializeBehaviour Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        /// <summary>
        /// The app ID.
        /// </summary>
        /// <remarks></remarks>
        [SerializeField]
        public string AppID;
        /// <summary>
        /// The app Key.
        /// </summary>
        /// <remarks></remarks>
        [SerializeField]
        public string AppKey;
        /// <summary>
        /// The site.
        /// </summary>
        /// <remarks>
        /// If <see cref="ServerUrl"/> is set, this attribute will be ignored.
        /// </remarks>
        [SerializeField]
        public Kii.Site Site;
        /// <summary>
        /// The API timeout in seconds.
        /// </summary>
        /// <remarks>
        /// The default value is 30 seconds.
        /// </remarks>
        [SerializeField]
        private int apiTimeout = 30;
        public int ApiTimeout
        {
            get
            {
                return this.apiTimeout;
            }
            private set
            {
                this.apiTimeout = value;
            }
        }

        /// <summary>
        /// The server URL.
        /// </summary>
        /// <remarks>
        /// This attribute is intended for use in debug purposes.
        /// Use <see cref="Site"/> instead.
        /// ex.) https://api.kii.com/api
        /// </remarks>
        #if DEBUG
        [SerializeField]
        #endif
        public string ServerUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="KiiCorp.Cloud.Storage.KiiInitializeBehaviour"/> class.
        /// </summary>
        /// <remarks>
        /// This constructor is called by UnityGameEngine. Do not use it from your application.
        /// </remarks>
        public KiiInitializeBehaviour ()
        {
        }

        /// <summary>
        /// Initializes the KiiCloudStorageSDK and KiiAnalyticsSDK.
        /// </summary>
        /// <remarks>
        /// This method is called by UnityGameEngine. Do not use it from your application.
        /// </remarks>
        public virtual void Awake()
        {
            if (!KiiInitializeBehaviour.isInitialized)
            {
                KiiInitializeBehaviour.isInitialized = true;
                DontDestroyOnLoad(this);
                INSTANCE = this;
                if (!Utils.IsEmpty(this.ServerUrl))
                {
                    Kii.Initialize(this.AppID, this.AppKey, this.ServerUrl,
                        new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
                    KiiAnalytics.Initialize(this.AppID, this.AppKey, this.ServerUrl, this.GetDeviceID(),
                        new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
                }
                else
                {
                    Kii.Initialize(this.AppID, this.AppKey, this.Site,
                        new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
                    KiiAnalytics.Initialize(this.AppID, this.AppKey, ToAnalyticsSite(this.Site), this.GetDeviceID(),
                        new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
                }
                WWWRequestLooper.ApiTimeout = this.ApiTimeout;
                StartCoroutine(WWWRequestLooper.RunLoop());
            }
        }
        /// <summary>
        /// Switchs the app.
        /// </summary>
        /// <param name="appId">AppID.</param>
        /// <param name="appKey">AppKey.</param>
        /// <param name="site">Site.</param>
        /// <remarks>
        /// This method is for internal use only. Do not use it from your application.
        /// </remarks>
        public void SwitchApp(string appId, string appKey, Kii.Site site)
        {
            Kii.Initialize(appId, appKey, site,
                new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
            KiiAnalytics.Initialize(appId, appKey, ToAnalyticsSite(site), this.GetDeviceID(),
                new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
            this.AppID = appId;
            this.AppKey = appKey;
            this.Site = site;
            this.ServerUrl = null;
        }
        /// <summary>
        /// Switchs the app.
        /// </summary>
        /// <param name="appId">AppID.</param>
        /// <param name="appKey">AppID.</param>
        /// <param name="serverUrl">Server URL.</param>
        /// <remarks>
        /// This method is for internal use only. Do not use it from your application.
        /// </remarks>
        public void SwitchApp(string appId, string appKey, string serverUrl)
        {
            Kii.Initialize(appId, appKey, serverUrl,
                new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
            KiiAnalytics.Initialize(appId, appKey, serverUrl, this.GetDeviceID(),
                new KiiUnityClientFactoryImpl(), new KiiAsyncUnityClientFactoryImpl());
            this.AppID = appId;
            this.AppKey = appKey;
            this.ServerUrl = serverUrl;
        }
        private string GetDeviceID()
        {
            string deviceId = PlayerPrefs.GetString(PREFS_KEY_DEVICE_ID, null);
            if (deviceId == null || deviceId.Length == 0)
            {
                deviceId = Guid.NewGuid().ToString();
                PlayerPrefs.SetString(PREFS_KEY_DEVICE_ID, deviceId);
                PlayerPrefs.Save();
            }
            return deviceId;
        }
        private KiiAnalytics.Site ToAnalyticsSite(Kii.Site site)
        {
            switch (site)
            {
            case Kii.Site.JP:
                return KiiAnalytics.Site.JP;
            case Kii.Site.US:
                return KiiAnalytics.Site.US;
            case Kii.Site.CN:
                return KiiAnalytics.Site.CN;
            case Kii.Site.SG:
                return KiiAnalytics.Site.SG;
            case Kii.Site.CN3:
                return KiiAnalytics.Site.CN3;
            case Kii.Site.EU:
                return KiiAnalytics.Site.EU;
            }
            throw new ArgumentException("Invalid Site");
        }
    }
}
