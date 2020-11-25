using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    GameObject mainMenu;
    GameObject controlScheme;
    GameObject levelSelect;
    GameObject levelButtons;
    GameObject credits;

    void Start()
    {
        mainMenu = GameObject.Find("MainMenu");
        controlScheme = GameObject.Find("ControlScheme");
        levelSelect = GameObject.Find("LevelSelectPanel");
        levelButtons = GameObject.Find("LevelSelectButtons");
        credits = GameObject.Find("Credits");
        mainMenu.SetActive(true);
        controlScheme.SetActive(false);
        levelSelect.SetActive(false);
        levelButtons.SetActive(false);
        credits.SetActive(false);
    }
    
    public void OpenScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void ControlScheme()
    {
        mainMenu.SetActive(false);
        controlScheme.SetActive(true);
    }

    public void LevelSelect()
    {
        mainMenu.SetActive(false);
        levelSelect.SetActive(true);
        levelButtons.SetActive(true);
    }

    public void Credits()
    {
        mainMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        mainMenu.SetActive(true);

        if (levelSelect.activeInHierarchy)
        {
            levelSelect.SetActive(false);
            levelButtons.SetActive(false);
        }
    }

    public void QuitGame() {
		Application.Quit();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9"))
        {
            mainMenu.SetActive(true);

            if (controlScheme.activeInHierarchy)
            {
                controlScheme.SetActive(false);
            }

            if (levelSelect.activeInHierarchy)
            {
                levelSelect.SetActive(false);
                levelButtons.SetActive(false);
            }

            if (credits.activeInHierarchy)
            {
                credits.SetActive(false);
            }
        }        
    }
}
