using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrSubscriptionClass : MonoBehaviour
{
    public static ctrSubscriptionClass instance;
    public Transform items;
    public GameObject panel;
    public GameObject congrats;
    float timerItems = 0;
    // Start is called before the first frame update
    void Awake() {
        //panel.SetActive(true);
        setScale();
    }

    void Start() {

        Debug.Log("ctrSubscriptionClass start");

        instance = this;

        if (staticClass.subscriptionsEnable) {
            if (staticClass.scenePrev != "menu" && staticClass.scenePrev != "level menu")
                //if (staticClass.scenePrev.Substring(0, 5) == "level" || staticClass.scenePrev == "level menu")
                if (staticClass.getLanguage() == 1 && ctrProgressClass.progress["vip"] != 1) panel.SetActive(true);
        }

        panel.transform.Find("content/spider/skin5").GetComponent<Animator>().Play("spider hi");

    }

    // Update is called once per frame
    void Update()
    {
        timerItems += Time.deltaTime;
        if (timerItems > Random.Range(0.5f, 4)) {
            timerItems = 0;
            //point
            //StartCoroutine(playBlikItem(items.GetChild(Random.Range(0, 6)).GetChild(0).GetChild(1)));
            StartCoroutine(playBlikItem(items.GetChild(Random.Range(0, 3)).GetChild(0).GetChild(1)));
        }

    }

    private IEnumerator playBlikItem(Transform obj) {
        for (int i = 0; i < 30; i ++) {
            yield return StartCoroutine(staticClass.waitForRealTime(0.01f));
            obj.localPosition = new Vector3(35 * i/2 - 140, 18 + (i/2 - 10) -50, 0);
        }
        //GetComponent<Animation>().Play("star 4");


    }

    public void getReward () {
        // { "berry1",1},{ "berry2",0},{ "berry3",0},{ "berry4",0},{ "berry5",0},
        //{ "hat1",1},{ "hat2",0},{ "hat3",0},{ "hat4",0},{ "hat5",0},
        //{ "skin1",1},{ "skin2",0},{ "skin3",0},{ "skin4",0},{ "skin5",0},

        for (int i = 2; i <= 5; i++) {
            ctrProgressClass.progress["berry" + i]++;
            ctrProgressClass.progress["hat" + i]++;
            ctrProgressClass.progress["skin" + i]++;
        }
        ctrProgressClass.progress["vip"] = 1;
        ctrProgressClass.saveProgress();

        panel.SetActive(false);
        congrats.SetActive(true);

    }


    void setScale() {
        float currentRes = (float)Screen.width / Screen.height;
        if (currentRes >= ctrResolutionClass.minRes) return;

        float increase = ctrResolutionClass.minRes / currentRes;
        float desrease = currentRes / ctrResolutionClass.minRes;
        items.transform.Find("vip_week/ps").localScale *= desrease;
        items.transform.localScale = new Vector3(desrease, desrease, desrease);
        //Debug.Log("---------------"+camera);

        //Debug.Log("---------------  Screen.width " + Screen.width);
        //Debug.Log("--------------- currentRes " + currentRes);
        Debug.Log("--------------- increase " + increase);
        Debug.Log("--------------- desrease " + desrease);
    }
}
