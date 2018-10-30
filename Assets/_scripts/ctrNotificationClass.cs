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
using NotificationServices = UnityEngine.iOS.NotificationServices;
using NotificationType = UnityEngine.iOS.NotificationType;
using UnityEngine.Networking;
using UnityEngine.iOS;

public class ctrNotificationClass: MonoBehaviour
{

	//bool tokenSent;
	public static ctrNotificationClass instance = null;


	void Start()
    {
		if (instance != null) {
			Destroy(gameObject);
			return;
		}
		instance = this;
		DontDestroyOnLoad(gameObject);

		Debug.Log("ctrNotificationClass start");
        cancelNotification();
		//local notification for iOS
		//tokenSent = false;

		NotificationServices.RegisterForNotifications(
			NotificationType.Alert |
			NotificationType.Badge |
			NotificationType.Sound);
	}

	private void Update()
	{
#if UNITY_IOS
		//local notification for iOS
		/*
		if (!tokenSent) {
			byte[] token = NotificationServices.deviceToken;
			if (token != null) {
				// send token to a provider
				string hexToken = "%" + System.BitConverter.ToString(token).Replace('-', '%');
				UnityWebRequest.Get("http:/example.com?token=" + hexToken).SendWebRequest();
				tokenSent = true;
			}
		}*/
#endif
	}

    public void OnApplicationFocus(bool flag)
    {
        Debug.Log("ctrNotificationClass OnFocus: " + flag);
        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
        if (!flag)
        {

            sendNotifers();


        }


    }
	public void OnApplicationPause(bool flag)
	{
		Debug.Log("ctrNotificationClass OnPause: " + flag);

	}

	void OnApplicationQuit()
    {
        Debug.Log("OnApplicationQuit");

        sendNotifers();
    }


    public void sendNotifers()
    {
        //sale notifer
        Debug.Log("sendNotifers");
        lsSaleClass.setTimerSale();
        lsSaleClass.setSale();
        cancelNotification();
        var delay = new TimeSpan();
        if (lsSaleClass.timerStartSale > DateTime.Now)
        {
            delay = lsSaleClass.timerStartSale - DateTime.Now;
            var h = DateTime.Now.Add(delay).Hour;
            if (h < 10) delay = delay.Add(new TimeSpan(10 - h, 0, 0));
            //Debug.Log("notifer 1 delay: " + delay);
            var type = ctrProgressClass.progress["firstPurchase"] == 1 ? "payers" : "free";
            sendNotification(1, delay, "", Localization.Get("notiferTitleSale") + Localization.Get("sale_" + (ctrProgressClass.progress["sale"] + 1) + "_" + type), new Color32(0xff, 0x44, 0x44, 255));
            Debug.Log(Localization.Get("notiferTitleSale") + Localization.Get("sale_" + (ctrProgressClass.progress["sale"] + 1) + "_" + type));
        }
        //daily notifer
        delay = DateTime.Parse("12:00:00") - DateTime.Now;
        if (delay < new TimeSpan(0)) delay = DateTime.Parse("12:00:00").AddDays(1) - DateTime.Now;
        //Debug.Log("daily notifer: " + delay);
        sendNotification(2, delay, "", Localization.Get("notiferTitleDay"), new Color32(0xff, 0x44, 0x44, 255));

        //daily 3 notifer
        delay = DateTime.Parse("12:00:00").AddDays(2) - DateTime.Now;
        if (delay < new TimeSpan(0)) delay = DateTime.Parse("12:00:00").AddDays(3) - DateTime.Now;
        //Debug.Log("daily notifer 3: " + delay);
        sendNotification(4, delay, "", Localization.Get("notiferTitleDay3"), new Color32(0xff, 0x44, 0x44, 255));

        //daily 7 notifer
        delay = DateTime.Parse("12:00:00").AddDays(6) - DateTime.Now;
        if (delay < new TimeSpan(0)) delay = DateTime.Parse("12:00:00").AddDays(7) - DateTime.Now;
        //Debug.Log("daily notifer 7: " + delay);
        sendNotification(5, delay, "", Localization.Get("notiferTitleDay7"), new Color32(0xff, 0x44, 0x44, 255));

        //daily 14 notifer
        delay = DateTime.Parse("12:00:00").AddDays(13) - DateTime.Now;
        if (delay < new TimeSpan(0)) delay = DateTime.Parse("12:00:00").AddDays(14) - DateTime.Now;
        //Debug.Log("daily notifer 14: " + delay);
        sendNotification(6, delay, "", Localization.Get("notiferTitleDay14"), new Color32(0xff, 0x44, 0x44, 255));

        //energy notifer
        lsEnergyClass.checkEnergy(true);
        if (lsEnergyClass.energy < lsEnergyClass.maxEnergy && !lsEnergyClass.energyInfinity)
        {
            //var start = new DateTime(2015, 1, 1).AddSeconds(ctrProgressClass.progress["energyTime"]);
            //var end = start.AddSeconds((lsEnergyClass.maxEnergy - lsEnergyClass.energy) * lsEnergyClass.costEnergy);
            //delay = end - DateTime.UtcNow;
            delay = new TimeSpan(0,0,
                ctrProgressClass.progress["energyTime"] + lsEnergyClass.maxEnergy*lsEnergyClass.costEnergy -
                    (int) DateTime.Now.TotalSeconds());
            //Debug.Log("notifer energy delay: " + delay);
            sendNotification(3, delay,"", Localization.Get("notiferTitleEnergy"), new Color32(0xff, 0x44, 0x44, 255));

        }

        //free coins notifer
        if (ctrProgressClass.progress["freeCoinsDate"] > 0)
        {
            //delay = lsSaleClass.timerStartSale - DateTime.Now;
            if (ctrProgressClass.progress["freeCoinsDate"] > DateTime.Now.TotalSeconds()) delay = DateTime.Now.AddSeconds(ctrProgressClass.progress["freeCoinsDate"] - DateTime.Now.TotalSeconds()) - DateTime.Now;
            else delay = new TimeSpan(0, 0, 0);
            if (delay.TotalSeconds == 0) delay = delay.Add(new TimeSpan(4, 0, 0));

            var h = DateTime.Now.Add(delay).Hour;
            if (h < 10) delay = delay.Add(new TimeSpan(10 - h, 0, 0));

            Debug.Log("free coins notifer: " + delay);
            sendNotification(7, delay, "", Localization.Get("notiferTitleCoins"), new Color32(0xff, 0x44, 0x44, 255));


        }
        
        //fix test
        delay = new TimeSpan(0, 0, 0);
        delay = delay.Add(new TimeSpan(0, 0, 30));
        sendNotification(8, delay, "", "TEST", new Color32(0xff, 0x44, 0x44, 255));
        Debug.Log("notifer test: " );
        
    }

	private void sendNotification(int id, TimeSpan delay, string title, string text, Color32 color)
	{
		var notificationParams = new NotificationParams {
			Id = id,
			Delay = delay,
			Title = title,
			Message = text,
			//Ticker = "Ticker",
			Sound = true,
			Vibrate = true,
			Light = true,
			SmallIcon = NotificationIcon.Bell,
			SmallIconColor = new Color(0.5f, 0.5f, 0.5f),
			LargeIcon = "app_icon"
		};

		NotificationManager.SendCustom(notificationParams);
		//NotificationManager.SendWithAppIcon(delay, title, text, new Color(0, 0.6f, 1), NotificationIcon.Bell);
		//NotificationManager.Send(delay, title, text, new Color(0, 0.6f, 1));
		//LocalNotification.SendNotification(id, delay, title, text, color);

#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			UnityEngine.iOS.LocalNotification notification = new UnityEngine.iOS.LocalNotification();
			notification.fireDate = DateTime.Now.Add( delay);
			notification.alertAction = "Alert";
			notification.alertBody = text;
			notification.hasAction = false;
			NotificationServices.ScheduleLocalNotification(notification);


		}
#endif
	}
    private void cancelNotification()
    {
        NotificationManager.CancelAll();
		//LocalNotification.ClearNotifications();
#if UNITY_IOS
		NotificationServices.CancelAllLocalNotifications();
		NotificationServices.ClearLocalNotifications();
#endif
	}
}
