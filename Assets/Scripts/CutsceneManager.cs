﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour {
	public PlayableDirector director;

    void Start() {
        
    }

    void Update() {
        if (director.time > 28.4) { // If cutscene done, move to level 1 (28 seconds)
			SceneManager.LoadScene("Level 1");
		}
    }
}