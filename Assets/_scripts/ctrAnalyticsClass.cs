using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Facebook.Unity;
using UnityEngine;
//using LocalyticsUnity;
using GameAnalyticsSDK;
//using Firebase;
using Assets.SimpleAndroidNotifications;

//for local notification iOS
using UnityEngine.Networking;
#if UNITY_IOS
using UnityEngine.iOS;
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
#endif
public class ctrAnalyticsClass: MonoBehaviour
{
    public static ctrAnalyticsClass instance = null;
    private int sessionTimeout = 60 * 5;


    public static List<string> developerIds = new List<string>
    { "15779554", "303171231", "4929221", "786955", "305568333", "51066050", "212234350", "100009826471037","100007730714188", "100004274864226", "790297741122714", "558610410993", "572497357200", "582889450510", "537314172230", "44697922", "534060256361" };

    public static List<float> ageGroups = new List<float> { 0, 10, 18, 24, 35, 56 };
    public static List<float> paymentGroups = new List<float> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 30 };
    public static List<float> revenueGroups = new List<float> { 0, 0.6F, 1, 1.5F, 2, 3, 5, 10, 20, 30 };
    //public static List<int> levelGroups = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,  };
    public static List<float> sessionGroups = new List<float> { 0, 1, 2, 3, 4, 5, 10, 25, 50, 100, 200  };
    public static List<float> friendGroups = new List<float> { 0, 1, 5, 10, 25, 50, 100 };

    //bool tokenSent;
    public static string lastAction = "";

    void Awake()
    {
        //fixnow - false for publish
        Debug.unityLogger.logEnabled = true;
        //Debug.unityLogger.logEnabled = false;

        //Localytics.Upload();
        if (FB.IsInitialized)
        {
            FB.ActivateApp();
        }
        else
        {
            //Handle FB.Init
            FB.Init(() => {
                FB.ActivateApp();
            });
        }
		GameAnalytics.Initialize();

		//Firebase.FirebaseApp.Create();


	}
	// Use this for initialization
	void Start()
    {



        /*
                Localytics.Upload();
                Localytics.LoggingEnabled = true;
                Localytics.RegisterForAnalyticsEvents();
                Localytics.RegisterForMessagingEvents();
                Localytics.TagEvent("click");
                Localytics.RegisterForAnalyticsEvents();
                Localytics.RegisterForLocationEvents();
                Localytics.RegisterForMessagingEvents();
                Localytics.TestModeEnabled = true;
                */
         //Localytics.integrate(this, "API_KEY")
        // e.g. with preprocessor directives
#if !UNITY_EDITOR && (UNITY_IOS || UNITY_ANDROID)
           // Localytics.TagScreen("xxx");
           // Localytics.Upload();
#endif
        //LocalyticsUnity.Localytics.cre
        //Unity 2017

        Debug.Log("ctrAnalyticsClass start");
        try
        {
            //Debug.Log("Localytics SessionTimeoutInterval: " + sessionTimeout);
            //LocalyticsUnity.Localytics.SessionTimeoutInterval = sessionTimeout;
        }
        catch (Exception)
        {
            Debug.Log("Localytics SessionTimeoutInterval error");
            //throw;
        }

        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);


        startSession();

        //local notification for iOS
        //tokenSent = false;
#if UNITY_IOS
        NotificationServices.RegisterForNotifications(
			NotificationType.Alert |
			NotificationType.Badge |
			NotificationType.Sound);
#endif


        //AppsFlyer
        /* Mandatory - set your AppsFlyer’s Developer key. */
        Debug.Log("AppsFlyer init");
        AppsFlyer.setAppsFlyerKey("Ura5UVbFB3YXvaig2PnvPA");
        /* For detailed logging */
        //fixnow
        AppsFlyer.setIsDebug (true);
#if UNITY_IOS
   /* Mandatory - set your apple app ID
      NOTE: You should enter the number only and not the "ID" prefix */
   AppsFlyer.setAppID ("1440167499");
   AppsFlyer.trackAppLaunch ();
#elif UNITY_ANDROID
        /* Mandatory - set your Android package name */
        AppsFlyer.setAppID("com.evogames.feedthespider");
        /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
        AppsFlyer.init("Ura5UVbFB3YXvaig2PnvPA", "AppsFlyerTrackerCallbacks");
#endif
    }

    private void Update()
	{

	}
	public static void sendEvent(string nameEvent, Dictionary<string, string> attributes2, long purchase = 0)
    {
        //LocalyticsUnity.Localytics.TagEvent(nameEvent, attributes);
        string str = "";
        str += nameEvent + "\n";

        Dictionary<string, string> attributes = new Dictionary<string, string> (attributes2);
        attributes.Add("level", ctrProgressClass.progress["lastLevel"].ToString());
        var s = ctrProgressClass.progress["sessionCount"];
        if (s == 0) s = 1;
        attributes.Add("session_number", s.ToString());
        attributes.Add("social_id", ctrFbKiiClass.userId);
        attributes.Add("keys_count", ctrProgressClass.progress["gems"].ToString());
        if (!attributes.ContainsKey("energy_count")) attributes.Add("energy_count", lsEnergyClass.energy.ToString());
        foreach (var attr in attributes)
        {
            str += attr.Key + ": " + attr.Value + "\n";
        }
        //new
        string strForGA = nameEvent;
        string strForLog = "Event: " + nameEvent;
        Dictionary<string, object> parameters = new Dictionary<string, object>();
        //Firebase.Analytics.Parameter[] parametersFirebase = new Firebase.Analytics.Parameter[attributes.Count];
        int i = 0;
        foreach (KeyValuePair<string, string> param in attributes)
        {

            strForGA += ":" + param.Key.ToString() + "_"  + param.Value.ToString();
            strForLog += ", " + param.Key.ToString() + ": " + param.Value.ToString();
            parameters[param.Key] = (object)param.Value;
            //parametersFirebase[i] = new Firebase.Analytics.Parameter(param.Key.ToString(), param.Value);
                i++;
        }
        //Debug.Log(strForLog);
        // Log an event with multiple parameters, passed as a struct:

        //Firebase.Analytics.FirebaseAnalytics.LogEvent(
        //  nameEvent,
         // parametersFirebase);


		 GameAnalytics.NewDesignEvent(strForGA);

		if (FB.IsInitialized)
			FB.LogAppEvent(
	            nameEvent,
	            null,
	            parameters
	        );


        //end new



        Debug.Log(strForGA);

        Debug.Log(str);
        try
        {
            //LocalyticsUnity.Localytics.TagEvent(nameEvent, attributes, purchase);
        }
        catch (Exception e)
        {
            Debug.Log("Localytics TagEvent error");
            Debug.Log(e.Message);
            //throw;
        }

        //AppsFlyer
        AppsFlyer.trackRichEvent(nameEvent, attributes);
    }
    public static void sendProfileAttribute(string key, string value)
    {
        Debug.Log("sendProfileAttribute: " + key + " = " + value);
        try
        {
            //LocalyticsUnity.Localytics.SetProfileAttribute(key, value);
            //Firebase.Analytics.FirebaseAnalytics.SetUserProperty(key, value);
            GameAnalytics.SetCustomDimension02(key +": " + value);
        }
        catch (Exception)
        {
            Debug.Log("Localytics SetProfileAttribute error");
        }
    }
    public static void sendCustomDimension(int key, string value)
    {
        Debug.Log("sendCustomDimension: " + key + " = " + value);
        try
        {
            //LocalyticsUnity.Localytics.SetCustomDimension(key, value);
            //Firebase.Analytics.FirebaseAnalytics.SetUserProperty("Property_" + key.ToString(), value);
            GameAnalytics.SetCustomDimension01(key + ": " + value);
        }

        catch (Exception)
        {
            Debug.Log("Localytics sendCustomDimension error");
        }
    }

    //standart event OnApplicationFocus dont work. why?
    public void applicationFocus(bool flag)
    {
        Debug.Log("OnFocus: " + flag);
        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
        if (!flag)
        {

            //pause
            ctrProgressClass.progress["sessionEnd"] = (int) DateTime.Now.TotalSeconds();
            ctrProgressClass.progress["energyOnEndSession"] = lsEnergyClass.energy;
            ctrProgressClass.saveProgress();

            sendEvent("GameUnfocus", new Dictionary<string, string> { { "LastAction", ctrAnalyticsClass.lastAction }, { "Scene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name } });


        }
        else
        {
            //PlayerPrefs.DeleteKey("progress");
            //ctrProgressClass.getProgress();
            //Debug.Log("awake");

            startSession();
        }
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!flag)
        {
            //app resume
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }
            else
            {
                //Handle FB.Init
                FB.Init(() => {
                    FB.ActivateApp();
                });
            }
        }
    }
	public void OnApplicationPause(bool flag)
	{
		Debug.Log("OnPause: " + flag);

	}

	void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");
        ctrProgressClass.progress["sessionEnd"] = (int)DateTime.Now.TotalSeconds();
        ctrProgressClass.saveProgress();

    }

    public void startSession()
    {
        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();

        Debug.Log("session end: " + ctrProgressClass.progress["sessionEnd"]);
        Debug.Log("session end 2: " + (int)DateTime.Now.AddSeconds(-sessionTimeout).TotalSeconds());
        if (ctrProgressClass.progress["sessionEnd"] < (int) DateTime.Now.AddSeconds(-sessionTimeout).TotalSeconds())
        {
            lsEnergyClass.checkEnergy(true);
            if (ctrProgressClass.progress["sessionStart"] > 1)
            {
                Debug.Log("--------------------------------------------------------------------");
                sendEvent("Session_End", new Dictionary<string, string>
                {
                    {
                        "session_length",
                        (Mathf.Round((ctrProgressClass.progress["sessionEnd"] -
                                      ctrProgressClass.progress["sessionStart"])/60F*100)/100F).ToString()
                    },
                    {"level_play_count", ctrProgressClass.progress["levelPlayCount"].ToString()},
                    {"win_count", ctrProgressClass.progress["winCount"].ToString()},
                    {"energy_count", ctrProgressClass.progress["energyOnEndSession"].ToString()}
                });
            }
            Debug.Log("startSession");
            
            ctrProgressClass.progress["sessionCount"]++;
            //Debug.Log("sessionCount: " + ctrProgressClass.progress["sessionCount"]);
            sendCustomDimension(5, getGroup(ctrProgressClass.progress["sessionCount"], ctrAnalyticsClass.sessionGroups)); //sessionCount

            ctrProgressClass.progress["sessionStart"] = (int)DateTime.Now.TotalSeconds();
            ctrProgressClass.progress["sessionEnd"] = (int)DateTime.Now.TotalSeconds();
            ctrProgressClass.progress["levelPlayCount"] = 0;
            ctrProgressClass.progress["winCount"] = 0;

            sendEvent("Session_Start",
                new Dictionary<string, string>
                {
                    {"coins", ctrProgressClass.progress["coins"].ToString()},
                    {"energy", lsEnergyClass.energy.ToString()}
                });

            Debug.Log("analytics session count: " + ctrProgressClass.progress["sessionCount"]);
            Debug.Log("analytics groups count: " + ctrAnalyticsClass.sessionGroups.Count);
            Debug.Log("analytics session group: " + getGroup(ctrProgressClass.progress["sessionCount"], ctrAnalyticsClass.sessionGroups));

            Debug.Log("firstLaunch: " + ctrProgressClass.progress["firstLaunch"]);
            if (ctrProgressClass.progress["firstLaunch"] == 0)
            {
                sendEvent("First_Launch", new Dictionary<string, string>());
                ctrProgressClass.progress["firstLaunch"] = 1;
            }
        
            ctrProgressClass.saveProgress();
        } 
    }

    public static void sendAnalyticsAfterSocialLogin()
    {
        
    }

    public static string getGroup (float value, List<float> list  )
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (value < list[i])
            {
                if (value == list[i - 1]) return list[i - 1].ToString();
                else return list[i - 1] + " - " + list[i];
            }
        }
        return list[list.Count - 1] + "+";
    }

    //setCustomerFirstName
    public static void setCustomerFirstName(string name)
    {
        Debug.Log("setCustomerFirstName: " + name);
        try
        {
            //LocalyticsUnity.Localytics.SetCustomerFirstName(name);
        }
        catch (Exception)
        {
            Debug.Log("Localytics setCustomerFirstName error");
        }
    }
    //setCustomerLastName
    public static void setCustomerLastName(string name)
    {
        Debug.Log("setCustomerLastName: " + name);
        try
        {
            //LocalyticsUnity.Localytics.SetCustomerLastName(name);
        }
        catch (Exception)
        {
            Debug.Log("Localytics setCustomerLastName error");
        }
    }
    //setCustomerFullName
    public static void setCustomerFullName(string name)
    {
        Debug.Log("setCustomerFullName: " + name);
        try
        {
            //LocalyticsUnity.Localytics.SetCustomerFullName(name);
        }
        catch (Exception)
        {
            Debug.Log("Localytics setCustomerFullName error");
        }
    }
    //setCustomerEmail
    public static void setCustomerEmail(string mail)
    {
        Debug.Log("setCustomerEmail: " + mail);
        try
        {
            //LocalyticsUnity.Localytics.SetCustomerEmail(mail);
        }
        catch (Exception)
        {
            Debug.Log("Localytics setCustomerEmail error");
        }
    }


    public static void funnelStart(int step, string stepName ) {
        if (ctrProgressClass.progress["funnelStep"] < step) {
            ctrProgressClass.progress["funnelStep"] = step;
            ctrProgressClass.saveProgress();
            sendEvent("FunnelStart", new Dictionary<string, string> { { "step", step.ToString("00") + "_" + stepName } });
        }
    }
}
