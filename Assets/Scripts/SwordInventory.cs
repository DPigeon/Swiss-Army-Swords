using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwordInventory : MonoBehaviour {
    [SerializeField]
    GameObject InventoryPrefab = null;
    [SerializeField]
    GameObject RegularSwordPrefab = null;
    [SerializeField]
    GameObject FlameSwordPrefab = null;
    [SerializeField]
    GameObject IceSwordPrefab = null;
    [SerializeField]
    GameObject BrickSwordPrefab = null;
    [SerializeField]
    GameObject LightSwordPrefab = null;
    [SerializeField]
    GameObject GuitarSwordPrefab = null;

	Joystick swordJoystick;
    AudioSource selectSound;

    public List<GameObject> inventoryList = new List<GameObject>(); // Keep track of the inventory
    public int index;
    public bool switchSwords;
    float inventoryDistance = 80;
    bool level2part2;
    Scene scene;
    public bool dialogueActive;

    void Start()
    {
        AddSlot(0);
        index = 0;
        selectSound = transform.GetComponent<AudioSource>();
        Color color = new Color(0.368F, 0.96F, 0.13F);
        inventoryList[index].GetComponent<SpriteRenderer>().color = color;
        switchSwords = true;
        level2part2 = false;
    }

    void Update()
    {
        scene = SceneManager.GetActiveScene();
        addSwords();
        if (scene.name == "Level 2 Part 2" && !level2part2)
        {
            index = 3;
            Color color = new Color(0.368F, 0.96F, 0.13F);
            inventoryList[0].GetComponent<SpriteRenderer>().color = Color.white;
            inventoryList[3].GetComponent<SpriteRenderer>().color = color;
            level2part2 = true;
        }

        if (scene.name == "Cutscene" || scene.name == "FinalCutscene" || dialogueActive) // prevent input during cutscenes
        { }
        else
        {
            ControlInventory();
        }
    }
    private void addSwords()
    {
        if (scene.name == "Level 2" || scene.name == "Level 2 Puzzle")
        {
            if (inventoryList.Count == 1)
            {
                AddSlot(1);
                AddSlot(2);
            }
        }
        if (scene.name == "Level 2 Part 2" || scene.name == "Level 3")
        {
            if (inventoryList.Count == 1)
            {
                AddSlot(1);
                AddSlot(2);
                AddSlot(3);
            }
        }
        if (scene.name == "DragonBoss" || scene.name == "LavaDemon")
        {
            if (inventoryList.Count == 1)
            {
                AddSlot(1);
                AddSlot(2);
                AddSlot(3);
                AddSlot(4);
            }
        }
    }

    /* Handles the addition of inventory once new sword obtained */
    public void AddSlot(int swordNumber) {
        Vector2 position = new Vector2(0, 0); // Initial position for first sword
        if (inventoryList.Count == 0) // Initialize
        {
            ShowSword(inventoryList.Count, position);
        }
        else // Add new
        {
            float newPositionX = inventoryList[inventoryList.Count - 1].transform.localPosition.x + inventoryDistance;
            float newPositionY = inventoryList[inventoryList.Count - 1].transform.localPosition.y;
            position = new Vector2(newPositionX, newPositionY);
            ShowSword(swordNumber, position);
        }
        GameObject slot = Instantiate(InventoryPrefab, position, Quaternion.identity) as GameObject;
        slot.transform.SetParent(gameObject.transform, false);
        slot.transform.localScale = new Vector2(37.5f, 37.5f);
        inventoryList.Add(slot);
    }

    private void ShowSword(int number, Vector3 pos)
    {
        // We have to find the order of the swords we are getting
        GameObject swordPrefab = null;
        if (number == 0)
            swordPrefab = RegularSwordPrefab;
        if (number == 1) // Brick Sword
            swordPrefab = BrickSwordPrefab;
        if (number == 2) // Ice sword
            swordPrefab = IceSwordPrefab;
        if (number == 3) // Light sword
            swordPrefab = LightSwordPrefab;
        if (number == 4) // Fire sword
            swordPrefab = FlameSwordPrefab;
        if (number == -1) // Guitar sword
            swordPrefab = GuitarSwordPrefab;
        var newSword = Instantiate(swordPrefab, pos, Quaternion.identity);
        newSword.transform.SetParent(gameObject.transform, false);
        newSword.transform.localScale = new Vector2(20, 20);
    }

    /* Controlling UI of the inventory */
    private void ControlInventory()
    {
        if (inventoryList.Count > 1 && switchSwords)
        {
            Color color = new Color(0.368F, 0.96F, 0.13F); // Set green color to show sword equipped
            if (Input.mouseScrollDelta.y > 0) // mouse scroll up
            {
                ScrollUp();
            }
            if (Input.mouseScrollDelta.y < 0) // mouse scroll down
            {
                ScrollDown();
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) // press 1
            {
                inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
                index = 0;
                inventoryList[index].GetComponent<SpriteRenderer>().color = color;
                selectSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && inventoryList.Count >= 2) // press 2 if player has at least 2 swords
            {
                inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
                index = 1;
                inventoryList[index].GetComponent<SpriteRenderer>().color = color;
                selectSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && inventoryList.Count >= 3)
            {
                inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
                index = 2;
                inventoryList[index].GetComponent<SpriteRenderer>().color = color;
                selectSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && inventoryList.Count >= 4)
            {
                inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
                index = 3;
                inventoryList[index].GetComponent<SpriteRenderer>().color = color;
                selectSound.Play();
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && inventoryList.Count == 5)
            {
                inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
                index = 4;
                inventoryList[index].GetComponent<SpriteRenderer>().color = color;
                selectSound.Play();
            }
        }
    }
	
	public void ScrollUp() {
		Color color = new Color(0.368F, 0.96F, 0.13F);
		inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
		index++;
		if (index == inventoryList.Count)
			index = 0;
		inventoryList[index].GetComponent<SpriteRenderer>().color = color;
		selectSound.Play();
	}
	
	public void ScrollDown() {
		Color color = new Color(0.368F, 0.96F, 0.13F);
		inventoryList[index].GetComponent<SpriteRenderer>().color = Color.white;
		index--;
		if (index == -1)
			index = inventoryList.Count - 1;
		inventoryList[index].GetComponent<SpriteRenderer>().color = color;
		selectSound.Play();
	}
}
