using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ctrResolutionClass : MonoBehaviour {
    //float minRes = 0.562f; // 480x854
    float minRes = 0.55f; 

    void Awake() {
        setScale();

    }
	
    
    void setScale () {
        float currentRes = (float) Screen.width / Screen.height;
        if (currentRes >= minRes) return;

        float increase = minRes / currentRes;
        float desrease = currentRes / minRes;

        Camera camera = GetComponent<Camera>();
        //Debug.Log("---------------"+camera);
        if (camera != null) camera.orthographicSize *= increase;
        //Debug.Log("---------------  Screen.width " + Screen.width);
        //Debug.Log("--------------- currentRes " + currentRes);
        //Debug.Log("--------------- increase " + increase);
        //Debug.Log("--------------- desrease " + desrease);

        switch (name) {
            case "button market": //start menu
                transform.position = new Vector3(transform.position.x * desrease, transform.position.y, 1);
                break;
            case "button settings": //start menu
                transform.position = new Vector3(transform.position.x * desrease, transform.position.y, 1);
                break;
            case "gradients": //map
            case "back ice folder": 
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * increase, 1);
                break;
            case "back forest folder":
            case "back": //transition
            case "back rock":
            case "back desert folder":
                transform.localScale = new Vector3(transform.localScale.x * increase, transform.localScale.y * increase, 1);
                break;
            case "tutorial dream colliders":
                transform.position = new Vector3(0, (minRes - currentRes) * 2300, 1);
                break;
            default:
                transform.localScale = new Vector3(transform.localScale.x * desrease, transform.localScale.y * desrease, 1);
                break;

        }
    }

}
