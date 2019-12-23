using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileActions : MonoBehaviour {
	GameObject player;
	GameObject activeSword;

    void Start() {
        player = GameObject.Find("Player");
		GetCurrentSword();
    }
	
	void GetCurrentSword() { // Used to get the current active sword
		for (int i = 0; i < player.transform.childCount; i++) {
			if (player.transform.GetChild(i).gameObject.activeSelf == true) {
				activeSword = player.transform.GetChild(i).gameObject;
			}
		}
	}

    void Update() {
        GetCurrentSword();
    }
	
	public void Attack() {
		activeSword.GetComponent<Sword>().MobileAttackButton();
	}
	
	public void Ability() {
		activeSword.GetComponent<Sword>().MobileAbilityButton();
	}
	
}
