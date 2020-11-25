using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    Camera mainCamera;
    GameObject ui;
    GameObject pauseMenu;
    GameObject controlScheme;
    GameObject buttons;
    GameObject regularDescription;
    GameObject iceDescription;
    GameObject brickDescription;
    GameObject lightDescription;
    GameObject flameDescription;
    List<int> possessions;
    Music music;
    public bool paused;
    Scene scene;

    void Start()
    {
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
        ui = GameObject.Find("UI Canvas");
        pauseMenu = GameObject.Find("MainPauseMenu");
        controlScheme = GameObject.Find("ControlScheme");
        buttons = GameObject.Find("PauseMenuButtons");
        regularDescription = GameObject.Find("RegularSwordDescription");
        iceDescription = GameObject.Find("IceSwordDescription");
        brickDescription = GameObject.Find("BrickSwordDescription");
        lightDescription = GameObject.Find("LightSwordDescription");
        flameDescription = GameObject.Find("FlameSwordDescription");
        possessions = GameObject.Find("Player").GetComponent<Player>().swordPossessions;
        music = GameObject.Find("Music").GetComponent<Music>();
        pauseMenu.GetComponent<Canvas>().worldCamera = mainCamera;
        controlScheme.GetComponent<Canvas>().worldCamera = mainCamera;
        buttons.GetComponent<Canvas>().worldCamera = mainCamera;
        scene = SceneManager.GetActiveScene();
        ui.SetActive(true);
        pauseMenu.SetActive(false);
        controlScheme.SetActive(false);
        buttons.SetActive(false);
        regularDescription.SetActive(false);
        iceDescription.SetActive(false);
        brickDescription.SetActive(false);
        lightDescription.SetActive(false);
        flameDescription.SetActive(false);
    }

    void Update()
    {
        if (pauseMenu.activeInHierarchy)
        {
            if (possessions.Contains(0))
            {
                regularDescription.SetActive(true);
            }
            if (possessions.Contains(1))
            {
                brickDescription.SetActive(true);
            }
            if (possessions.Contains(2))
            {
                iceDescription.SetActive(true);
            }
            if (possessions.Contains(3))
            {
                lightDescription.SetActive(true);
            }
            if (possessions.Contains(4))
            {
                flameDescription.SetActive(true);
            }
        }

        if (Input.GetMouseButtonDown(0) && controlScheme.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            buttons.SetActive(true);
            controlScheme.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown("joystick button 9"))
        {
            if (scene.name == "Cutscene" || scene.name == "FinalCutscene") // prevent input during cutscenes
            { }
            else
            { 
                if (!pauseMenu.activeInHierarchy && controlScheme.activeInHierarchy)
                {
                    pauseMenu.SetActive(true);
                    buttons.SetActive(true);
                    controlScheme.SetActive(false);
                }
                else if (!pauseMenu.activeInHierarchy)
                {
                    paused = true;
                    PauseGame();
                }
                else if (pauseMenu.activeInHierarchy && !controlScheme.activeInHierarchy)
                {
                    paused = false;
                    ContinueGame();
                }
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        ui.SetActive(false);
        pauseMenu.SetActive(true);
        buttons.SetActive(true);
    }

    public void ContinueGame()
    {
        paused = false;
        Time.timeScale = 1;
        ui.SetActive(true);
        pauseMenu.SetActive(false);
        buttons.SetActive(false);
    }
    
    public void RestartLevel()
    {
        if (pauseMenu.activeInHierarchy)
        {
            string name = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(name);
            Time.timeScale = 1;
        }
    }

    public void ExitLevel()
    {
        if (pauseMenu.activeInHierarchy)
        {
            string name = "MainMenu";
            music.mainMenu = false;
            music.cutscene = false;
            SceneManager.LoadScene(name);
            Time.timeScale = 1;
        }
    }

    public void ControlScheme()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            buttons.SetActive(false);
            controlScheme.SetActive(true);
        }
    }

}
