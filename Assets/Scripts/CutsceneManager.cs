using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour {
	public PlayableDirector director;
	string sceneName;
    Music music;

    void Start() {
        Scene currentScene = SceneManager.GetActiveScene(); // To know which level
		sceneName = currentScene.name;
        music = GameObject.Find("Music").GetComponent<Music>();
    }

    void Update() {
		if (sceneName == "Cutscene") {
			if (director.time > 28.4) { // If cutscene done, move to level 1 (28 seconds)
				SceneManager.LoadScene("Level 1");
			}
			if (Input.GetButton("SkipCutscene")) { // Skip cutscene by pressing "s"
				director.time = 28.0;
			}
		} else if (sceneName == "FinalCutscene") {
			if (director.time > 50.5) { // If cutscene done, move to menu
				SceneManager.LoadScene("MainMenu");
                music.mainMenu = false;
			}
		}
    }
}
