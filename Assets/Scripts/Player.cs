using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public HitpointBar playerHPBar;
	
	Joystick joystick;

    [SerializeField] float playerSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] LayerMask platformLayerMask;
    [SerializeField] float jumpDuration;
    [SerializeField] float timeJumpFactor;
    [SerializeField] AudioClip[] jumpsSounds;
    [SerializeField] public AudioClip[] attacksSounds;
    [SerializeField] public AudioClip[] hurtSounds;

    public AudioSource audioSource;

    bool pickingUpSword;
    bool moving;
    public bool grounded;
    bool falling;
    bool jumping;
    private bool isHurt;
    float jumpTimeElapsed;
    Rigidbody2D rb;
    Animator playerAnimator;
    Animator swordAnimator;
    public Vector2 facingDirection;
    HitpointBar hitpointBar;
    Collider2D playerCollider;

    SwordInventory inventory;
    GameObject inventoryGO;
    public List<Transform> swords = new List<Transform>();
    public List<int> swordPossessions = new List<int>();
    int activeSwordIndex;
    GameObject pauseMenu;
    Scene scene;

    float isHurtTime = 0.6f;
    float isHurtTimer = 0;

    string sceneName;
    GameObject brickSword;
    bool brickSwordHeld;
    public bool dialogueActive;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        moving = false;
        grounded = false;
        falling = false;
        hitpointBar = GameObject.Find("HitpointBar").GetComponent<HitpointBar>();
		joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();

        audioSource = GetComponent<AudioSource>();

        inventoryGO = GameObject.Find("InventoryManager");
        inventory = inventoryGO.GetComponent<SwordInventory>();
        swordPossessions.Add(0);
        addSwords();
        getInventorySwords();
        activeSwordIndex = inventory.index;
        pauseMenu = GameObject.Find("PauseMenu");
        playerHPBar = GameObject.Find("HitpointBar").GetComponent<HitpointBar>();

        Scene currentScene = SceneManager.GetActiveScene(); // To know which level
        sceneName = currentScene.name;
        brickSwordHeld = false; // for sword pickup only
    }

    private void addSwords()
    {
        scene = SceneManager.GetActiveScene();
        if (scene.name == "Level 2" || scene.name == "Level 2 Puzzle")
        {
            swordPossessions.Add(1);
            swordPossessions.Add(2);
        }
        if (scene.name == "Level 2 Part 2" || scene.name == "Level 3")
        {
            swordPossessions.Add(1);
            swordPossessions.Add(2);
            swordPossessions.Add(3);
        }
        if (scene.name == "DragonBoss" || scene.name == "LavaDemon")
        {
            swordPossessions.Add(1);
            swordPossessions.Add(2);
            swordPossessions.Add(3);
            swordPossessions.Add(4);
        }
    }

    private void getInventorySwords()
    {
        // Only have regular sword so set that as active
        if (swordPossessions.Count == 1)
        {
            swords.Add(transform.GetChild(0));
            activeSwordIndex = 0;
            return;
        }

        // get all the sword prefabs within the player game object
        for (int i = 0; i < transform.childCount; i++)
        {
            // if sword is already in inventory, do not add it again
            if (swordPossessions.Contains(i) && !swords.Contains(transform.GetChild(i)))
            {
                swords.Add(transform.GetChild(i));
            }
            if (transform.GetChild(i).gameObject.activeInHierarchy)
            {
                // Keep track of which sword in the array is active
                activeSwordIndex = i;
            }
        }
    }

    private bool findSwordInInventory(Transform curSword)
    {
        foreach (GameObject sword in inventory.inventoryList)
        {
            string name = curSword.name.Replace(" ", "");
            Debug.Log(name);
            if (name == sword.name)
                return true;
        }
        return false;
    }

    void Update()
    {
        if (scene.name == "Cutscene" || scene.name == "FinalCutscene" || dialogueActive) // prevent input during cutscenes
        { }
        else
        {
            MovePlayer();
            SwitchSwords();
        }
        isGrounded();
        CheckFalling();
        playerAnimator.SetBool("isMoving", moving);
        playerAnimator.SetBool("isGrounded", grounded);
        playerAnimator.SetBool("isFalling", falling);
        playerAnimator.SetBool("isPickingUpSword", pickingUpSword);
        playerAnimator.SetBool("isHurt", isHurt);
        activeSwordIndex = inventory.index;
        checkHurt();
        DeathHandler();
    }

    private void DeathHandler()
    {
        if (hitpointBar.PlayerHealth <= 0)
        {
            playerAnimator.SetTrigger("death");
            StartCoroutine(resetAfterSeconds(2));
        }
    }

    IEnumerator resetAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void checkHurt()
    {
        if (isHurt)
        {
            if(isHurtTimer > isHurtTime)
            {
                isHurt = false;
                isHurtTimer = 0;
            }
            isHurtTimer += Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.Contains("SwordDrop") && !pickingUpSword)
        {
            // Cannot switch swords until inventory is updated
            inventory.switchSwords = false;
            if (GameObject.Find("Brick Sword") != null) // if brick sword is active
            {
                brickSword = GameObject.Find("Brick Sword");
                brickSword.SetActive(false);
                brickSwordHeld = true;
            }
            // Hide the player's held sword
            SpriteRenderer heldSwordSR = new SpriteRenderer();
            SpriteRenderer[] srs = GetComponentsInChildren<SpriteRenderer>();
            // GetComponentsInChildren includes the parent object, so if the parent object exists
            // Then we've found a proper child component i.e. the held sword
            foreach (SpriteRenderer sr in srs)
                if (sr.gameObject.transform.parent != null)
                    heldSwordSR = sr;
            if (heldSwordSR != null)
                heldSwordSR.enabled = false; // Hide it
            pickingUpSword = true;
            moving = false;
            // Hold sword above head - sorta buggy when you jump and collect it

            col.transform.parent = transform;
            col.gameObject.transform.position = new Vector2(transform.position.x, transform.position.y + 1);
            col.gameObject.GetComponentInChildren<Animator>().enabled = false;

            swordPossessions.Add(SwordId(col.gameObject));

            StartCoroutine(HandleSwordPickup(col.gameObject, heldSwordSR));
        }
        if(col.name == "Death")
        {
            StartCoroutine(resetAfterSeconds(1));
        }

    }
    void OnTriggerStay2D(Collider2D col)
    {
        if (col.tag == "Lever")
        {
            if (Input.GetButtonDown("Fire2"))
            {
                col.GetComponent<LeverDoorController>().toggle();
            }
        }
    }

     void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Platforms")  // or if(gameObject.CompareTag("YourWallTag"))
        {
            rb.velocity = Vector3.zero;
        }
    }


    private IEnumerator WaitAndPickup(GameObject swordGO, SpriteRenderer heldSwordSR)
    {
        // Will force a wait before the player can continue playing
        // Mainly so there's a pick-up animation that's held for 2 seconds
        // before the sword is added to the inventory and they can continue
        yield return StartCoroutine(HandleSwordPickup(swordGO, heldSwordSR));
    }

    private IEnumerator HandleSwordPickup(GameObject swordGO, SpriteRenderer heldSwordSR)
    {
        yield return new WaitForSeconds(2.0f); // Will hold the pose for 2 seconds.

        // After this timer, add the new sword to the inventory
        SwordInventory si = GameObject.FindObjectOfType<SwordInventory>();
        if (si != null)
        {
            si.AddSlot(SwordId(swordGO));
            Destroy(swordGO);
        }
        // Stop the animation and make the player's original sword reappear in their hand
        if (heldSwordSR != null)
            heldSwordSR.enabled = true;
        pickingUpSword = false;
        getInventorySwords(); // Get the inventory of swords again to account for new one
        if (brickSwordHeld) // if brick sword is held upon pickup
        {
            brickSword.SetActive(true);
        }
        inventory.switchSwords = true; // Able to switch swords once inventory is updated
    }

    private void MovePlayer()
    {
        moving = false;
        if (!pickingUpSword && !pauseMenu.GetComponent<Pause>().paused)
        {
            if (Input.GetButton("Left") || joystick.Horizontal <= -0.2F)
            {
                transform.Translate(-Vector2.right * playerSpeed * Time.deltaTime);
                transform.localScale = new Vector3(-1, 1, 1);
                facingDirection = -transform.right;
                moving = true;
            }
            if (Input.GetButton("Right") || joystick.Horizontal >= 0.2F)
            {
                transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
                transform.localScale = new Vector3(1, 1, 1);
                facingDirection = transform.right;
                moving = true;
            }

            //jump handling
            if (grounded && Input.GetButtonDown("Jump") || grounded && joystick.Vertical >= 0.5F)
            {
                jumpTimeElapsed = 0;
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumping = true;
                makeJumpNoise();
            }
            if (jumping && Input.GetButton("Jump") || jumping && joystick.Vertical >= 0.0F)
            {
                timeJump();
            }
            if (Input.GetButtonUp("Jump") || joystick.Vertical >= 0.51F)
            {
               jumping = false;
            }
        }
    }

    private void makeJumpNoise()
    {
        audioSource.clip = jumpsSounds[UnityEngine.Random.Range(0, jumpsSounds.Length)];
        audioSource.time = 0.2f;
        audioSource.Play();
    }

    private void timeJump()
    {
        if(jumpTimeElapsed < jumpDuration) {
            rb.AddForce(Vector2.up * jumpForce * (1-jumpTimeElapsed ) * Time.deltaTime * timeJumpFactor,  ForceMode2D.Impulse);
            jumpTimeElapsed += Time.deltaTime;
        }
        
    }

    private void CheckFalling()
    {
        falling = rb.velocity.y < 0.0f;
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    public bool IsHurt
    {
        get { return isHurt; }
        set { isHurt = value; }
    }

    public Rigidbody2D PlayerRigidBody()
    {
        return rb;
    }

    private void SwitchSwords()
    {
        if (inventory.switchSwords && swords.Count > 1) // Making sure the player has more than one sword)
        {
            if (Input.mouseScrollDelta.y > 0) // mouse scroll up
                {
                    swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                    swords[activeSwordIndex].gameObject.SetActive(false); // Disable current sword

                    // Switch to next sword
                    if (activeSwordIndex + 1 == swords.Count)
                    {
                        activeSwordIndex = 0;
                    }
                    else
                    {
                        activeSwordIndex = activeSwordIndex + 1;
                    }

                    swords[activeSwordIndex].gameObject.SetActive(true); // Re-enable the (selected) sword
                }
            else if (Input.mouseScrollDelta.y < 0) // mouse scroll down
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false); // Disable current sword

                // Switch to next sword
                if (activeSwordIndex - 1 == -1)
                {
                    activeSwordIndex = swords.Count - 1;
                }
                else
                {
                    activeSwordIndex = activeSwordIndex - 1;
                }

                swords[activeSwordIndex].gameObject.SetActive(true); // Re-enable the (selected) sword
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) // press 1
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false);
                swords[0].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) && swords.Count >= 2) // press 2 if player has at least 2 swords
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false);
                swords[1].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) && swords.Count >= 3)
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false);
                swords[2].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) && swords.Count >= 4)
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false);
                swords[3].gameObject.SetActive(true);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) && swords.Count == 5)
            {
                swords[activeSwordIndex].GetComponent<Sword>().cooldownTimer = 0;
                swords[activeSwordIndex].gameObject.SetActive(false);
                swords[4].gameObject.SetActive(true);
            }
        }
    }

    private int SwordId(GameObject sword)
    {
        string name = sword.name;
        if (name.Contains("Brick"))
            return 1;
        if (name.Contains("Ice"))
            return 2;
        if (name.Contains("Light"))
            return 3;
        if (name.Contains("Flame"))
            return 4;
        if (name.Contains("Guitar"))
            return -1;
        return 0;
    }

    private void isGrounded()
    {
        float extraHeightText = 0.2f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size * 0.9f, 0f, Vector2.down, extraHeightText, platformLayerMask);

        Debug.DrawRay(playerCollider.bounds.center + new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightText), Color.green);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, 0), Vector2.down * (playerCollider.bounds.extents.y + extraHeightText), Color.green);
        Debug.DrawRay(playerCollider.bounds.center - new Vector3(playerCollider.bounds.extents.x, playerCollider.bounds.extents.y + extraHeightText), Vector2.right * (playerCollider.bounds.extents.x + extraHeightText), Color.green);

        grounded = raycastHit.collider != null;
    }
}

