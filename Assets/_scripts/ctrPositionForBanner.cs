using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ctrPositionForBanner : MonoBehaviour
{
    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("USER_GROUP_BANNER", -1) == 1) GetComponent<UIWidget>().bottomAnchor.Set(0, 400);
    }
}
