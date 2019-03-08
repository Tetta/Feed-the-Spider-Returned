using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrSubscriptionClass : MonoBehaviour
{
    public static ctrSubscriptionClass instance;
    public Transform items;
    public GameObject panel;
    float timerItems = 0;
    // Start is called before the first frame update
    void Awake() {
        //panel.SetActive(true);
    }

    void Start() {
        //StartCoroutine(startStar());
        Debug.Log("ctrSubscriptionClass start");
        Debug.Log(staticClass.scenePrev);
        Debug.Log(staticClass.getLanguage());
        //if (instance != null) {
        //    Destroy(gameObject);
        //    return;
        //}
        //DontDestroyOnLoad(gameObject);
        instance = this;

#if UNITY_IOS
        if (staticClass.scenePrev != "menu" && staticClass.scenePrev != "level menu")
            //if (staticClass.scenePrev.Substring(0, 5) == "level" || staticClass.scenePrev == "level menu")
           if (staticClass.getLanguage() == 1) panel.SetActive(true);
#endif

    }

    // Update is called once per frame
    void Update()
    {
        timerItems += Time.deltaTime;
        if (timerItems > Random.Range(0.5f, 4)) {
            timerItems = 0;
            StartCoroutine(playBlikItem(items.GetChild(Random.Range(0, 6)).GetChild(0).GetChild(1)));
        }
        
    }

    private IEnumerator playBlikItem(Transform obj) {
        for (int i = 0; i < 20; i ++) {
            yield return StartCoroutine(staticClass.waitForRealTime(0.01f));
            obj.localPosition = new Vector3(35 * i/2 - 140, 18 + (i/2 - 10) -50, 0);
        }
        //GetComponent<Animation>().Play("star 4");


    }
}
