using System;
using UnityEngine;
using System.Collections;
#if UNITY_ANDROID || UNITY_IOS

#endif
using System.Collections.Generic;
using Facebook.Unity;
//using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;


//using Odnoklassniki;
using Object = System.Object;

using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
using UnityEngine.Advertisements;
//2740765

public class ctrAdClass : MonoBehaviour, IRewardedVideoAdListener, IInterstitialAdListener/*, INonSkippableVideoAdListener*/ {

    public static ctrAdClass instance = null;
    public static string adStarted = "";

    private Dictionary<string, object> adsAttributes = new Dictionary<string, object>
    {
        {"name", "ad coins"},
        {"type", "rewarded"},
        {"status", "viewed"}
    };

    private int showAdLevelCounter = 0;
   // public GoogleMobileAds.Api.InterstitialAd interstitialAdMob;

    //for myTarget
    private readonly System.Object _syncRoot = new System.Object();
	#if !UNITY_IOS
    //private Mycom.Target.Ads.InterstitialAd rewardedMyTarget;
    //private Mycom.Target.Ads.InterstitialAd imgMyTarget;
	#endif


#if UNITY_ANDROID
    private uint rewardedMyTargetId = 92777; 
    private uint imgMyTargetId = 92774;
#elif UNITY_IPHONE
    private uint rewardedMyTargetId = 92793; 
    private uint imgMyTargetId = 92790; 

#endif
    public static bool rewardedMyTargetLoaded = false;
    public static bool imgMyTargetLoaded = false;
    public bool needSetRewardMyTarget = false;
    public static long timeFailed = 0; //for analytics only
    public static bool adDisplayed;
    public static bool unityAdsLoaded;

    private int levelAdCounter = 0;

#if UNITY_ANDROID
    public static string appKeyAppodeal = "696005dece72ab367077e595ed5b245f52a52044528a8f80";
#else
    public static string appKeyAppodeal = "3a4bc268f2c33545ea93819a1f82e439b9d34a9bd0a23174";
#endif
    List<string> availableNetworks = new List<string>() { "adcolony", "admob", "amazon_ads", "applovin", "appnext", "avocarrot", "chartboost", "facebook", "flurry", "inmobi", "inner-active", "ironsource", "mailru", "mmedia", "mopub", "mobvista", "ogury", "openx", "pubnative", "smaato", "startapp", "tapjoy", "unity_ads", "vungle", "yandex" };

    //point
    //for review AppStore interstitial after 5 level (off after "06/30/2019")
    string adAfterDate = "06/30/2019";

    public void Start()
    {


        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
        //if (ctrProgressClass.progress["ok"] == 1 && OK.IsLoggedIn)
        //    disableNetworks();
        if (ctrProgressClass.progress["firstTimeAd"] == 0) initUnityAds();
    }

    public static void initUnityAds() {
        //Advertisement.Initialize("2740765");
    }
    public static bool isUnityAdsReady() {
        //return Advertisement.IsReady("video");
        return false;
    }
    public static void showUnityAds() {
        //Advertisement.Show("video");
        ctrProgressClass.progress["firstTimeAd"] = 1;
    }
    public void initAppodeal () {
        Appodeal.initialize(appKeyAppodeal, Appodeal.REWARDED_VIDEO | Appodeal.INTERSTITIAL /*| Appodeal.NON_SKIPPABLE_VIDEO*/);
        
        Appodeal.setRewardedVideoCallbacks(this);
        Appodeal.setInterstitialCallbacks(this);
        //Appodeal.setNonSkippableVideoCallbacks(this);
    }

    public void ShowRewardedAd()
    {
        if (adStarted == "ad coins" && ctrProgressClass.progress["tutorialAdCoins"] < 3)
        {
            Debug.Log("tutorialAdCoins < 3");
            ctrProgressClass.progress["tutorialAdCoins"] = 3;
            GameObject.Find("/root/static/ad coins/hand").SetActive(false);
        }

        adsAttributes["name"] = adStarted;
        adsAttributes["type"] = "rewarded";
        Debug.Log("click ShowRewardedAd");
        
        if (isAdReady(Appodeal.REWARDED_VIDEO))
        {
            Appodeal.show(Appodeal.REWARDED_VIDEO);
        }
        else
        {
            //if loading failed, send analytics event
            adsAttributes["status"] = "notready";
            //fix убрать таймер
            //if (isTimeToSendFailedAdAnalytics())
                ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
            //adDontReadyMenu
            if (initLevelMenuClass.instance != null) initLevelMenuClass.instance.adDontReadyMenu.SetActive(true);
            else if (SceneManager.GetActiveScene().name.Substring(0, 5) == "level") GameObject.Find("/default level/gui/ad dont ready menu").transform.GetChild(0).gameObject.SetActive(true);
        }
        //fix - uncomment for test
        //setReward(); 
    }

    void setReward()
    {
        if (adStarted == "button ad energy")
        {
            lsEnergyClass.energy = 1;
            //energyGO
            if (GameObject.Find("root/static/energy") != null)
                GameObject.Find("root/static/energy").SendMessage("OnApplicationFocus", true);
            GameObject.Find("energy menu/panel with anim/energy").SendMessage("OnApplicationFocus", true);
            if (marketClass.instance.gameObject.activeSelf)
                GameObject.Find("/market/inventory/market menu/bars/energy").SendMessage("OnApplicationFocus", true);

        }
        else if (adStarted == "ad coins")
        {
            ctrProgressClass.progress["coins"] += 50;
            //coinsLabel
            AdCoinsTimerClass.counter++;
            ctrProgressClass.progress["adCoinsDate"] =
                (int) DateTime.Now.AddSeconds(AdCoinsTimerClass.interval).TotalSeconds();
            AdCoinsTimerClass.timer = DateTime.Now.AddSeconds(AdCoinsTimerClass.interval);
            //for test off GameObject.Find("root/static/coins/coinsLabel").GetComponent<UILabel>().text =  ctrProgressClass.progress["coins"].ToString();
            /*
            initLevelMenuClass.instance.rewardMenu.SetActive(true);
            initLevelMenuClass.instance.rewardMenu.transform.GetChild(0)
                .GetChild(5)
                .GetChild(0)
                .GetChild(3)
                .GetChild(3)
                .GetComponent<UILabel>()
                .text = "50";
                */
            ctrAnalyticsClass.sendEvent("Coins", new Dictionary<string, object> {{"detail", "video"}, {"coins", "50"}});
            initLevelMenuClass.instance.coinsMenu.SetActive(false);

            //ps coins
            var t = GameObject.Find("/root/static/ps coins");
            t.SetActive(false);
            t.SetActive(true);
        }
        else if (adStarted == "dreamShowAd")
        {
            staticClass.levelAdViewed = staticClass.levelRestartedCount + 3;
            //сохраняем dream
            var nameScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            var p = ctrProgressClass.progress[nameScene + "_dream"];

            if (p == 0 && initLevelMenuClass.levelDemands == 0)
                ctrProgressClass.progress[nameScene + "_dream"] = 1;
            else if (p == 0 && initLevelMenuClass.levelDemands == 1)
                ctrProgressClass.progress[nameScene + "_dream"] = 2;
            else
                ctrProgressClass.progress[nameScene + "_dream"] = 3;
            ctrProgressClass.saveProgress();
            gHintClass.initDream();
        }
        else if (adStarted == "hintShowAd") {
            Debug.Log("setReward hintShowAd");
            gHintClass.useHint();
        }
        ctrProgressClass.saveProgress();
        adStarted = "";
    }

    public static bool isAdReady(int type)
    {
        if (Appodeal.isLoaded(type)) return true;
        else Appodeal.initialize(appKeyAppodeal, type);
        return false;
    }

    public bool ShowLevelAd(string buttonName)
    {
        Debug.Log("ShowLevelAd: " + buttonName);

        //if OK, ad rate /2
        float r = UnityEngine.Random.value;
        //if (OK.IsLoggedIn && ctrProgressClass.progress["ok"] == 1 && r > 0.5F) return false;

        int adAfterLevel = 5;
        if (staticClass.adHard) adAfterLevel = 1;
//#if UNITY_IOS
 //       if (DateTime.Now < DateTime.Parse(adAfterDate)) adAfterLevel = 5;
//#else
        if (DateTime.Now < DateTime.ParseExact(adAfterDate, "MM/dd/yyyy", null)) adAfterLevel = 5;
        Debug.Log("DateTime.Now: " + DateTime.Now);
        Debug.Log("DateTime: " + DateTime.ParseExact(adAfterDate, "MM/dd/yyyy", null));
        //#endif
        if (ctrProgressClass.progress["firstPurchase"] == 0 && ctrProgressClass.progress["currentLevel"] >= adAfterLevel && (!staticClass.rateUsLevels.Contains(ctrProgressClass.progress["currentLevel"])))
        {
            bool flag = false;

            if (((initLevelMenuClass.levelDemands == 0 && buttonName == "button play 0") ||
                    (initLevelMenuClass.levelDemands == 1 && buttonName == "button play 1")) &&
                SceneManager.GetActiveScene().name != "level menu")
                buttonName = "restart";
            if (staticClass.levelRestartedCount < 2) staticClass.levelAdViewed = 0;
            if (buttonName == "restart")
            {
                var mod = (1 + staticClass.levelRestartedCount)%3;
                if (staticClass.levelAdViewed > 1)
                    mod = (1 + staticClass.levelRestartedCount + staticClass.levelAdViewed)%3;
                if (mod == 0) flag = true;
            }
            else
            {
                //if (buttonName == "button next level" ||
                //     (initLevelMenuClass.levelDemands == 0 && buttonName == "button play 1") ||
                //    (initLevelMenuClass.levelDemands == 1 && buttonName == "button play 0")     )
                //    staticClass.levelAdViewed = 0;
                if (SceneManager.GetActiveScene().name != "level menu" && SceneManager.GetActiveScene().name != "menu" &&
                    staticClass.levelAdViewed == 0 && lsEnergyClass.energy != 0) flag = true;

            }
            adsAttributes["name"] = "level";
            adsAttributes["type"] = "interstitial";
            Debug.Log("ShowLevelAd: " + buttonName);
            Debug.Log("ShowLevelAd levelRestartedCount: " + staticClass.levelRestartedCount);
            Debug.Log("ShowLevelAd flag: " + flag);
            Debug.Log("ShowLevelAd scene: " + SceneManager.GetActiveScene().name);
            Debug.Log("ShowLevelAd levelDemands: " + initLevelMenuClass.levelDemands);
            Debug.Log("ShowLevelAd staticClass.levelAdViewed: " + staticClass.levelAdViewed);
            Debug.Log("ShowLevelAd lsEnergyClass.energy: " + lsEnergyClass.energy);

            if (flag)
            {
                Debug.Log("need ShowLevelAd");
                //staticClass.setApplicationFocus(false); for test
                //if (levelAdCounter % 3 == 2 && isAdReady(Appodeal.NON_SKIPPABLE_VIDEO)) {
                if (false) {
                    ctrAdClass.adStarted = "level";
                    adsAttributes["type"] = "non_skippable";
                    //Appodeal.
                    Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
                    levelAdCounter++;
                    return true;
                }
                else if (isAdReady(Appodeal.INTERSTITIAL))
                {
                    ctrAdClass.adStarted = "level";
                    adsAttributes["type"] = "interstitial";
                    //pause level
                    /* пока не будем
                    staticClass.isTimePlay = Time.timeScale;
                    Time.timeScale = 0;
                    Debug.Log("Time.timeScale: " + Time.timeScale);
                    */

                    Appodeal.show(Appodeal.INTERSTITIAL);
                    levelAdCounter++;
                    return true;
                    //GameObject.Find("default level/gui/pause").SendMessage("OnPress", false);
                }
                else
                {
                    adsAttributes["status"] = "notready";
                    Debug.Log("ShowLevelAd fail");
                    //fix убрать таймер
                    //if (isTimeToSendFailedAdAnalytics())
                        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
                }

            }


        }

        return false;
    }
    //may be needed
    //staticClass.setApplicationFocus(false);
    //if (ctrProgressClass.progress["music"] == 1) musicClass.instance.GetComponent<AudioSource>().mute = false;

    public static bool isTimeToSendFailedAdAnalytics()
    {
        //5 minutes
        if (timeFailed < DateTime.Now.AddMinutes(-5).TotalSeconds())
        {
            timeFailed = DateTime.Now.TotalSeconds();
            return true;
        }
        return false;
    }

    public IEnumerator coroutineSetReward()
    {

        yield return StartCoroutine(staticClass.waitForRealTime(0.5F));
        setReward();
    }
    /*
    public void showInterstitialFirstTime () {
        Debug.Log("showInterstitialFirstTime");
        Debug.Log("session: " + ctrProgressClass.progress["sessionCount"]);

        if (ctrProgressClass.progress["sessionCount"] == 0 && ctrProgressClass.progress["firstTimeAd"] == 0) {
            adsAttributes["name"] = "first_fime";
            adsAttributes["type"] = "interstitial";
            ctrProgressClass.progress["firstTimeAd"] = 1;
            Appodeal.show(Appodeal.INTERSTITIAL);
        }
    }
    */
    //--------------------------------- Appodeal -----------------------------------------------------------------------------------
    /*
    void disableNetworks()
    {
        foreach (string value in availableNetworks)
        {
            if (value == "mailru") continue;
            Appodeal.disableNetwork(value);
        }
    }
    */

#region Rewarded Video callback handlers
    public void onRewardedVideoLoaded(bool flag)
    {
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "loaded";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("Video loaded");
    }
    public void onRewardedVideoFailedToLoad()
    {
        Appodeal.initialize(appKeyAppodeal, Appodeal.REWARDED_VIDEO);
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "failed";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("Rewarded Video failed");
    }
    public void onRewardedVideoShown()
    {
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "shown";
        ctrAnalyticsClass.lastAction = adsAttributes["name"] + "RewardedShown";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("Video shown");
    }
    public void onRewardedVideoClosed(bool finished) {
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "closed";
        ctrAnalyticsClass.lastAction = "";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("Video closed"); }

    public void onRewardedVideoFinished(double amount, string name)
    {
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "finished";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        instance.StartCoroutine(instance.coroutineSetReward());

        //setReward();
    }
    public void onRewardedVideoClicked()
    {
        adsAttributes["type"] = "rewarded";
        adsAttributes["status"] = "clicked";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onRewardedVideoClicked");
    }

    public void onRewardedVideoExpired ()
    {

    }
#endregion

#region Interstitial callback handlers
    public void onInterstitialClicked()
    {
        adsAttributes["type"] = "interstitial";
        adsAttributes["status"] = "clicked";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onRewardedVideoClicked");
    }
    public void onInterstitialFailedToLoad()
    {
        Appodeal.initialize(appKeyAppodeal, Appodeal.INTERSTITIAL);
        adsAttributes["type"] = "interstitial";
        adsAttributes["status"] = "failed";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("Video Interstitial failed");
    }

    public void onInterstitialShown()
    {
        adsAttributes["type"] = "interstitial";
        adsAttributes["status"] = "shown";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onInterstitialShown");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //if (adReward == "levelFinished") LevelFinishedGUI.instance.adShown();
        ctrAnalyticsClass.lastAction = "InterstitialShown";
        staticClass.setApplicationFocus(false);
        ctrAnalyticsClass.funnelStart(9, "level1_interstitial_shown");

    }
    public void onInterstitialFinished()
    {
        adsAttributes["type"] = "interstitial";
        adsAttributes["status"] = "finished";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onInterstitialFinished");
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        //if (adReward == "levelFinished") LevelFinishedGUI.instance.adShown();

    }
    public void onInterstitialLoaded(bool f)
    {
        //if (f) {
        //interstitialLoaded = true;
            adsAttributes["type"] = "interstitial";
            adsAttributes["status"] = "loaded";
            ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
            print("onInterstitialLoaded");
            //showInterstitialFirstTime();
        //}
    }


    public void onInterstitialClosed() {
        adsAttributes["type"] = "interstitial";
        adsAttributes["status"] = "closed";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        ctrAnalyticsClass.lastAction = "";
        staticClass.setApplicationFocus(true);

        print("onInterstitialClosed");
        ctrAnalyticsClass.funnelStart(10, "level1_interstitial_closed");

    }
    public void onInterstitialExpired()
    {

    }
#endregion

#region Non Skippable callback handlers
    /*
    public void onNonSkippableClicked() {
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "clicked";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onNonSkippableClicked");
    }
    public void onNonSkippableVideoFailedToLoad() {
        Appodeal.initialize(appKeyAppodeal, Appodeal.NON_SKIPPABLE_VIDEO);
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "failed";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onNonSkippableVideoFailedToLoad");
    }

    public void onNonSkippableVideoShown() {
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "shown";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onNonSkippableVideoShown");

        staticClass.setApplicationFocus(false);

    }
    public void onNonSkippableVideoFinished() {
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "finished";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onNonSkippableVideoFinished");


    }
    public void onNonSkippableVideoLoaded(bool f) {
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "loaded";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        print("onNonSkippableVideoLoaded");

    }


    public void onNonSkippableVideoClosed(bool finished) {
        adsAttributes["type"] = "non_skippable";
        adsAttributes["status"] = "closed";
        ctrAnalyticsClass.sendEvent("Advertisment", adsAttributes);
        staticClass.setApplicationFocus(true);

        print("onNonSkippableVideoClosed");
    }
    public void onNonSkippableVideoExpired() {

    }
    */
#endregion




}
