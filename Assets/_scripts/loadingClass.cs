using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEngine.UI;

public class loadingClass : MonoBehaviour {
	

	IEnumerator Start() {
        //void Start() {
        //if (SystemInfo.deviceModel == "iPhone8,4") Screen.SetResolution(640, 1136, true, 30);
        
        Debug.Log("start: " + Time.realtimeSinceStartup);
		Application.backgroundLoadingPriority = ThreadPriority.High;
        Debug.Log("loadingClass 1");
        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
        //показываем Unity Ads при первом запуске игры
        ctrAdClass.unityAdsLoaded = false;


        AsyncOperation async = SceneManager.LoadSceneAsync("menu");
        Debug.Log("loadingClass 2");

        ctrAdClass.instance. initAppodeal();
        //label.text = "Loading complete";
        Debug.Log("loadingClass 4");
        //yield return new WaitForSeconds(5);


        //Text label = GetComponent<Text>();
        while ( !async.isDone ){
            //Debug.Log(string.Format( "Loading {0}%", async.progress*100 ));
            //Debug.Log(Time.realtimeSinceStartup);
            //label.text = string.Format( "Loading {0}%", async.progress*100 ) ;
            Debug.Log(async.isDone);
            yield return null;
		}

        yield return async;

        //yield return null;

    }

    void Update () {
		if (Time.frameCount == 1) Debug.Log("update 1: " + Time.realtimeSinceStartup);
	
	}

}
