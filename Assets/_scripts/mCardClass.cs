﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using UnityEngine.SceneManagement;


public class mCardClass : MonoBehaviour {
	public string functionEnable = "";
	public string functionPress = "";
	public string functionDisable = "";

    // Use this for initialization
    void Start () {
		//StartCoroutine (startCard());	
	}

	
	// Update is called once per frame
	void Update () {
	
	}

	void OnEnable() {
		if (functionEnable != "") SendMessage (functionEnable);
	}	
	void OnPress(bool isPressed) {
		if (!isPressed && functionPress != "") SendMessage(functionPress, isPressed);
    }

	void OnDisable() {
		if (functionDisable != "") {
			for (int i = 0; i < 5; i++) 
			disableSkinPreview (transform.parent.parent.GetChild(1).GetChild(0).GetChild (i).gameObject, false);
		}
	}

    void enableCard() {
		//GetComponent<Animator> ().Stop ();
		//если куплен
        if (ctrProgressClass.progress.Count == 0) ctrProgressClass.getProgress();
        if (ctrProgressClass.progress[name] >= 1) {
            //frontside and backside
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else {
			transform.GetChild(0).gameObject.SetActive(false);
			transform.GetChild(1).gameObject.SetActive(true);

        }


        //если выбран текущий скин или шапка или ягода berryCurrent

        if (ctrProgressClass.progress[name.Substring(0, name.Length - 1) + "Current"] == int.Parse( name.Substring(name.Length - 1, 1)))
        {
            pressCard(false);
        }
        if (ctrProgressClass.progress[name] > 1)
        {
            transform.GetChild(0).GetChild(3).GetChild(3).gameObject.SetActive(true);
            transform.GetChild(0).GetChild(3).GetChild(3).GetChild(0).GetChild(0).GetComponent<UILabel>().text = ctrProgressClass.progress[name].ToString();
        }
    }

    void pressCard(bool isPressed) {
        if (!isPressed) {
			//stop all animation
            for (int i = 0; i < 5; i++) {
                Transform prevObject = transform.parent.GetChild(i);
                if (ctrProgressClass.progress[name] >= 1) prevObject.GetChild(3).gameObject.SetActive(false);
                /*
				if (prevObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card selected") ||
                    prevObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card select")
                    )
                    prevObject.GetComponent<Animator>().Play("default");
                    */
            }


            //start включаем preview текущий скин и выключаем все остальные
            string skinName = name;
            if (name.Substring(0, 3) == "hat") skinName = staticClass.currentSkin;
            Transform previewObj = transform.parent.parent.GetChild(1).GetChild(0);
            for (int i = 0; i < 5; i++) {
                if (previewObj.GetChild(i).name == skinName) {
					//previewObj.GetChild(i).gameObject.SetActive(true);
					disableSkinPreview (previewObj.GetChild (i).gameObject, true);

					 
					if (name.Length == 5) {
						previewObj.GetChild (i).GetComponent<Animator> ().Play ("spider hi");
						previewObj.GetChild (i).GetChild (1).GetChild (0).gameObject.GetComponent<AudioSource> ().Play();
					}
					if (name.Substring(0, 3) == "hat") {
						previewObj.GetChild (i).GetComponent<Animator> ().Play ("spider breath");
						//включаем текущую шапку и выключаем все остальные
                        for (int j = 0; j < 4; j++) {
							if (previewObj.GetChild (i).GetChild (0).GetChild (j).name == name) {
								previewObj.GetChild (i).GetChild (0).GetChild (j).gameObject.SetActive (true);
							} else {
								previewObj.GetChild (i).GetChild (0).GetChild (j).gameObject.SetActive (false);
							}
                        }
                    }

                }
                else { 
					disableSkinPreview (previewObj.GetChild (i).gameObject, false);
				}
            }
			//включаем описание скина или шапки
			for (int i = 0; i < 5; i++) {
			    if ("label " + name == previewObj.GetChild(i + 5).name)
			    {
			        previewObj.GetChild (i + 5).gameObject.SetActive (true);
                    //меняем количество бонусов в описании
			        if (i != 0 && ctrProgressClass.progress[name] != 0)
			        {
			            if (name.Substring(0, 3) == "hat") previewObj.GetChild(i + 5).GetChild(1).GetComponent<UILabel>().text = "+" + int.Parse(previewObj.GetChild(i + 5).GetChild(2).GetComponent<UILabel>().text) * ctrProgressClass.progress[name] + "%";
                        else if (name.Substring(0, 4) == "skin") previewObj.GetChild(i + 5).GetChild(1).GetComponent<UILabel>().text = "" + int.Parse(previewObj.GetChild(i + 5).GetChild(2).GetComponent<UILabel>().text) * ctrProgressClass.progress[name];
                        else previewObj.GetChild(i + 5).GetChild(1).GetComponent<UILabel>().text = "+" + int.Parse( previewObj.GetChild(i + 5).GetChild(2).GetComponent<UILabel>().text) * ctrProgressClass.progress[name];
			        }

			    }
				else previewObj.GetChild (i + 5).gameObject.SetActive (false);

			}

            //end включаем preview текущий скин и выключаем все остальные

            GetComponent<Animator>().Play("card select");
            //если куплен, то выбираем
            if (ctrProgressClass.progress[name] >= 1) {
                transform.GetChild(3).gameObject.SetActive(true);

                // = 1 и запись в static
                if (name.Substring(0, 4) == "skin") {
                    ctrProgressClass.progress["skinCurrent"] = int.Parse(name.Substring(4, 1));
                    staticClass.currentSkin = name;
                    staticClass.changeSkin();
                }
                else if (name.Substring(0, 3) == "hat") {
                    ctrProgressClass.progress["hatCurrent"] = int.Parse(name.Substring(3, 1));
                    staticClass.currentHat = name;
                    staticClass.changeHat();
                }
                else if (name.Substring(0, 5) == "berry") {
                    ctrProgressClass.progress["berryCurrent"] = int.Parse( name.Substring(5, 1));
                    staticClass.currentBerry = name;
                    staticClass.changeBerry();
                }
                ctrProgressClass.saveProgress();

				//выключаем get booster
				//transform.parent.parent.GetChild(1).GetChild(1).gameObject.SetActive(false);
            }
            //else
				//transform.parent.parent.GetChild(1).GetChild(1).gameObject.SetActive(true);
			
        }
    }

	void disableSkinPreview (GameObject skinPreview, bool flag) {
		if ((skinPreview.activeSelf && !flag) || (!skinPreview.activeSelf && flag)) {
			skinPreview.SetActive (flag);
			//skin
			//возврат изменений, сделанных анимацией "spider hi" (предотвращает баг если часто переключаешься)
			if (skinPreview.transform.rotation.z != 0) {
				
				skinPreview.transform.position = new Vector3 (0, 0, 0);
				skinPreview.transform.rotation = new Quaternion (0, 0, 0, 0);
				//legs
				skinPreview.transform.GetChild (9).localPosition = new Vector3 (-51, -71, 0);
				skinPreview.transform.GetChild (9).rotation = new Quaternion (0, 0, 0, 0);
				skinPreview.transform.GetChild (10).localPosition = new Vector3 (-67, -87, 0);
				skinPreview.transform.GetChild (10).rotation = new Quaternion (0, 0, 0, 0);
				skinPreview.transform.GetChild (11).localPosition = new Vector3 (-77.3F, -67.7F, 0);
				skinPreview.transform.GetChild (11).rotation = new Quaternion (0, 0, 0, 0);
				skinPreview.transform.GetChild (12).gameObject.SetActive (true);
				skinPreview.transform.GetChild (13).localPosition = new Vector3 (69, -87, 0);
				skinPreview.transform.GetChild (13).rotation = new Quaternion (0, 0, 0, 0);
				skinPreview.transform.GetChild (14).localPosition = new Vector3 (-76, -67.3F, 0);
				skinPreview.transform.GetChild (14).rotation = new Quaternion (0, 0, 0, 0);
				//smile
				skinPreview.transform.GetChild (30).localPosition = new Vector3 (0, 0, -10000);
				skinPreview.transform.GetChild (31).localPosition = new Vector3 (0, 0, -10000);
				//bandage
				if (skinPreview.name == "skin4") {
					skinPreview.transform.GetChild (32).localPosition = new Vector3 (1, 7, 0);
					skinPreview.transform.GetChild (32).localScale = new Vector3 (1, 1, 1);
				}
			}
		}

	}

    public IEnumerator openCard(bool flag)
    {
        if (flag) yield return StartCoroutine(staticClass.waitForRealTime(0.2F));
        else yield return null;
        foreach (var c in mBoosterClass.instance.openingCards)
        {
            UnityEngine.Debug.Log(c.Key);    
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card idle") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("start state"))
        {
            UnityEngine.Debug.Log("openingCards: " + mBoosterClass.instance.openingCards.Count);
            if (transform.childCount > 4)
            {
                GetComponent<Animator>().Play("card open epic");
                transform.GetChild(6).GetComponent<AudioSource>().Play();
                StartCoroutine(counterCard(1.1F, "exit open booster menu"));

            }
            else
            {
            GetComponent<Animator>().Play("card open");
            StartCoroutine(counterCard(0.5F, "exit open booster menu"));

            }
            GetComponent<AudioSource>().Play();

            
            //mBoosterClass.counterOpenCard++;
            //if (mBoosterClass.counterOpenCard >= mBoosterClass.openingCards.Count)
            var name = mBoosterClass.instance.openingCards.LastOrDefault().Key;

            //analytics
            if (name == "hints" || name == "webs" || name == "teleports" || name == "collectors")
            {
                ctrAnalyticsClass.sendEvent("Bonuses", new Dictionary<string, string>
                {
                    {"detail", "booster"},
                    {"name", name},
                    {"count", mBoosterClass.instance.openingCards[name].ToString()}
                });
            }
            if (name == "coins")
                ctrAnalyticsClass.sendEvent("Coins",
                    new Dictionary<string, string>
                    {
                        {"detail", "booster"},
                        {"coins", mBoosterClass.instance.openingCards[name].ToString()}
                    });


            //for play anim "block disable" on map 
            if (name == "gems") staticClass.keysBefore = ctrProgressClass.progress[name];

            //сохранение результата
            //UnityEngine.Debug.Log("mBoosterClass.openingCards.FirstOrDefault().Key: " + name);
            ctrProgressClass.progress[name] += mBoosterClass.instance.openingCards[name];



            mBoosterClass.instance.openingCards.Remove(name);
                //mBoosterClass.instance.openingCardsList.RemoveAt(0);
                mBoosterClass.instance.saveCards();
                ctrProgressClass.saveProgress();

            if (name == "gems" && SceneManager.GetActiveScene().name == "level menu" && staticClass.levelBlocks.ContainsKey(ctrProgressClass.progress["lastLevel"] + 1))
                GameObject.Find("level " + (ctrProgressClass.progress["lastLevel"] + 1)).GetComponent<lsLevelClass>().blockDisable();

            //update max energy labels if skin
            if (name.Length > 4 && name.Substring(0, 4) == "skin")
            {
                var e = lsEnergyClass.energy;
                //lsEnergyClass.maxEnergy += int.Parse(name.Substring(4, 1)) + 3;
                //UnityEngine.Debug.Log(lsEnergyClass.maxEnergy);
                if (SceneManager.GetActiveScene().name == "level menu")
                {
                    GameObject.Find("/root/static/energy").GetComponent<lsEnergyClass>().energyLabel.text = e.ToString();
                    GameObject.Find("/root/static/energy").GetComponent<lsEnergyClass>().energyMaxLabel.text = lsEnergyClass.maxEnergy.ToString();
                }
                if (marketClass.instance.gameObject.activeSelf)
                {
                    GameObject.Find("/market/inventory/market menu/bars/energy").GetComponent<lsEnergyClass>().energyLabel.text = e.ToString();
                    GameObject.Find("/market/inventory/market menu/bars/energy").GetComponent<lsEnergyClass>().energyMaxLabel.text = lsEnergyClass.maxEnergy.ToString();
                }
            }
            if (name == "coins")
            {
                if (SceneManager.GetActiveScene().name == "level menu") initLevelMenuClass.instance.coinsLabel.text = ctrProgressClass.progress["coins"].ToString();
                if (marketClass.instance.gameObject.activeSelf) GameObject.Find("/market/inventory/market menu/bars/coins/label coins").GetComponent<UILabel>().text = ctrProgressClass.progress["coins"].ToString();
            }
            if (name == "gems")
            {
                if (SceneManager.GetActiveScene().name == "level menu") initLevelMenuClass.instance.gemsLabel.text = ctrProgressClass.progress["gems"].ToString();

            }



        }
        else 
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card opened"))
        {
            transform.localPosition  = new Vector3(2000, 2000, 0);
            GetComponent<Animator>().Play("start state");
            
                UnityEngine.Debug.Log(mBoosterClass.instance.openingCards.Count);
                UnityEngine.Debug.Log(mBoosterClass.instance.openingCards.LastOrDefault().Key);
                UnityEngine.Debug.Log(mBoosterClass.instance);
                StartCoroutine(mBoosterClass.instance.transform.GetChild(2).Find(mBoosterClass.instance.openingCards.LastOrDefault().Key).GetComponent<mCardClass>().openCard(false));
            

        }



    }

	void openCardGift2(bool isPressed) {

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card idle") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("start state"))
        {
            if (transform.childCount > 4)
            {
                GetComponent<Animator>().Play("card open epic");
                transform.GetChild(6).GetComponent<AudioSource>().Play();

            }
            else GetComponent<Animator>().Play("card open");
			GetComponent<AudioSource> ().Play ();
			bool flag = true; 




			//перебор 3х карт, если хоть одна закрыта, то flag = false
			for (int i = 0; i < 3; i++) {
                //if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card idle") && transform.parent.GetChild (i).name != gameObject.name)

                if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card idle"))
                        flag = false;
			}

			if (flag) {
				transform.parent.parent.parent.GetChild(1).GetComponent<iClickClass>().functionPress = "closeMenu";
				transform.parent.parent.parent.GetChild(2).gameObject.SetActive(true);

			}





        }
        else
        {
            if (name == "card1") transform.parent.parent.parent.GetChild(1).GetComponent<iClickClass>().closeMenu();
            transform.localPosition = new Vector3(2000, 2000, 0);
        }

    }

	void openCardDaily(bool isPressed) {
		if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open")) {
			GetComponent<Animator>().Play("card open");
			GetComponent<AudioSource> ().Play ();

			bool flag = true; 
			//перебор 3х карт, если это первый клик, то сохраняется результат
			for (int i = 0; i < 3; i++) {
				if (transform.parent.GetChild (i).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open"))
					flag = false;
			}

			if (flag) {

				//название карты и количество
				string bonusName = name.Split(new Char[] { '_' })[0];
				int bonusCount = int.Parse(name.Split(new Char[] { '_' })[1]); 

				//сохранение результата
				if (bonusName == "hints" || bonusName == "webs" || bonusName == "teleports" || bonusName == "collectors" || bonusName == "coins") ctrProgressClass.progress[bonusName] += bonusCount;
				else if (bonusName == "energy") {
					ctrProgressClass.progress["energyTime"] -= bonusCount * lsEnergyClass.costEnergy;
                    lsEnergyClass.energy += bonusCount;
				}
				else {
					if (ctrProgressClass.progress[bonusName] == 0) ctrProgressClass.progress[bonusName] = 1;
					else
						//если есть, начислять монеты. поменять количество и добавить всплывающую иконку монет.
						ctrProgressClass.progress["coins"] += 100;
				}

		
				ctrProgressClass.saveProgress();
				StartCoroutine (openCardDailyAnother());
			}
		}
        else
        {
            transform.localPosition = new Vector3(2000, 2000, 0);
        }

    }

	private IEnumerator openCardDailyAnother(){
		yield return StartCoroutine(staticClass.waitForRealTime(0.7F));
		for (int i = 0; i < 3; i++) {
			transform.parent.GetChild (i).GetComponent<Animator>().Play("card open");
			GetComponent<AudioSource> ().Play ();

		}	
		//включаем exit daily menu
		transform.parent.parent.parent.GetChild(1).GetComponent<iClickClass>().functionPress = "closeMenu";
		transform.parent.parent.parent.GetChild(3).gameObject.SetActive(true);


	}
	private IEnumerator startCard(){
        //yield return StartCoroutine(staticClass.waitForRealTime(UnityEngine.Random.value * 2));
        yield return null;
        //if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open") && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open epic")) GetComponent<Animator> ().Play ("card idle");
        //if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open") && !GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card open epic")) GetComponent<Animator>().Play("card idle", -1, UnityEngine.Random.Range(-1, 1));
        GetComponent<Animator>().PlayInFixedTime("card idle", -1, UnityEngine.Random.Range(0, 10));

}
    private IEnumerator counterCard(float t, string exitMenuName)
    {
        int count = int.Parse( transform.GetChild(0).GetChild(3).GetChild(3).GetChild(0).GetComponent<UILabel>().text);
        yield return StartCoroutine(staticClass.waitForRealTime(t));

            if (exitMenuName != "exit gift menu" && mBoosterClass.instance.openingCards.Count == 0)
                transform.parent.parent.parent.parent.Find(exitMenuName).localPosition = new Vector3(0, 0, -1);
        

        if (count > 1)
        {
            var counterGO = transform.GetChild(0).GetChild(3).GetChild(3).gameObject;
            counterGO.SetActive(true);
            for (int i = 1; i <= 10; i++)
            {
                counterGO.transform.localScale = new Vector3( 1F + i/20F, 1F + i/20F, 1) ;
                counterGO.transform.GetChild(0).GetComponent<UILabel>().text =
                    (i*count/10).ToString();

                yield return StartCoroutine(staticClass.waitForRealTime(0.04F));

            }
            counterGO.transform.localScale = new Vector3(1, 1, 1);
        }


    }


    public IEnumerator openCardGift(bool flag)
    {
        if (flag) yield return StartCoroutine(staticClass.waitForRealTime(0.2F));
        else yield return null;

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card idle") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("start state"))
        {
            GetComponent<Animator>().Play("card open");
            StartCoroutine(counterCard(0.5F, "exit gift menu"));
            GetComponent<AudioSource>().Play();




        }
        else
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("card opened"))
        {
            transform.localPosition = new Vector3(2000, 2000, 0);
            GetComponent<Animator>().Play("start state");

            if (transform.GetSiblingIndex() - 1 >= 0) StartCoroutine(transform.parent.GetChild( transform.GetSiblingIndex() - 1).GetComponent<mCardClass>().openCardGift(false));
            else transform.parent.parent.parent.GetChild(1).GetComponent<iClickClass>().closeMenu();

        }



    }


}
