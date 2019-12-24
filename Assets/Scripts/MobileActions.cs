using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileActions : MonoBehaviour {
	GameObject player;
	GameObject activeSword;
	GameObject inventory;
	GameObject pauseMenu;
	GameObject ui;

    void Start() {
        player = GameObject.Find("Player");
		inventory = GameObject.Find("InventoryManager");
		ui = GameObject.Find("UI Canvas");
		pauseMenu = GameObject.Find("PauseMenu");
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
	
	public void Jump() {
		player.GetComponent<Player>().JumpMobile();
	}
	
	public void JumpPressed() {
		player.GetComponent<Player>().JumpButtonDown();
	}
	
	public void JumpReleased() {
		player.GetComponent<Player>().JumpButtonUp();
	}
	
	public void Ability() {
		activeSword.GetComponent<Sword>().MobileAbilityButton();
	}
	
	public void SwitchSwordsLeft() {
		player.GetComponent<Player>().SwitchSwordsDown();
		inventory.GetComponent<SwordInventory>().ScrollDown();
	}
	
	public void SwitchSwordsRight() {
		player.GetComponent<Player>().SwitchSwordsUp();
		inventory.GetComponent<SwordInventory>().ScrollUp();
	}
	
	public void PauseGame() {
		pauseMenu.GetComponent<Pause>().PauseGame();
	}
	
}
