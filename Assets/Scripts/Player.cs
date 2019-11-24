﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float jumpForce;
    [SerializeField] GameObject regularSword;
    [SerializeField] GameObject iceSword;
    [SerializeField] GameObject brickSword;
    [SerializeField] LayerMask platformLayerMask;

    bool pickingUpSword;
    bool moving;
    bool grounded;
    bool falling;
    Rigidbody2D player;
    Animator playerAnimator;
    Animator swordAnimator;
    Vector2 facingDirection;
    CapsuleCollider2D playerCollider;

    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        moving = false;
        grounded = false;
        falling = false;
    }

    public void ChangeToBossScene() {
        if (Input.GetButtonDown("ToBoss")) {
            SceneManager.LoadScene("PlayerBossInteraction");
        }
    }

    void Update()
    {
        isGrounded();
        MovePlayer();
        CheckFalling();
        SwitchSwords();
        ChangeToBossScene();
        playerAnimator.SetBool("isMoving", moving);
        playerAnimator.SetBool("isGrounded", grounded);
        playerAnimator.SetBool("isFalling", falling);
        playerAnimator.SetBool("isPickingUpSword", pickingUpSword);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.Contains("SwordDrop") && !pickingUpSword)
        {
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
            col.gameObject.transform.localPosition = new Vector2(transform.position.x, transform.position.y + 1);
            col.gameObject.GetComponentInChildren<Animator>().enabled = false;
            StartCoroutine(WaitAndPickup(col.gameObject, heldSwordSR));
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
    }

    private void MovePlayer()
    {
        moving = false;
        if (!pickingUpSword)
        {
            if (Input.GetButton("Left"))
            {
                transform.Translate(-Vector2.right * playerSpeed * Time.deltaTime);
                transform.localScale = new Vector2(-1, 1);
                facingDirection = -transform.right;
                moving = true;
            }
            if (Input.GetButton("Right"))
            {
                transform.Translate(Vector2.right * playerSpeed * Time.deltaTime);
                transform.localScale = new Vector2(1, 1);
                facingDirection = transform.right;
                moving = true;
            }

            if (grounded && Input.GetButtonDown("Jump"))
            {
                player.AddForce(Vector2.up * jumpForce);
            }

        }
    }

    private void CheckFalling()
    {
        falling = player.velocity.y < 0.0f;
    }

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

    private void SwitchSwords()
    {
        if (Input.GetKeyDown("tab"))
        {
            if (transform.name == "Player")
            {
                GameObject newPlayer = Instantiate(iceSword, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                if (facingDirection == (Vector2)(-transform.right))
                {
                    newPlayer.transform.localScale = new Vector2(-1, 1);
                    newPlayer.GetComponent<Player>().facingDirection = new Vector2(-1, 0);
                }
                Destroy(GameObject.Find("Player"));
            }
            if (transform.name == "Player Regular Sword(Clone)")
            {
                GameObject newPlayer = Instantiate(iceSword, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                if (facingDirection == (Vector2)(-transform.right))
                {
                    newPlayer.transform.localScale = new Vector2(-1, 1);
                    newPlayer.GetComponent<Player>().facingDirection = new Vector2(-1, 0);
                }
                Destroy(GameObject.Find("Player Regular Sword(Clone)"));
            }
            if (transform.name == "Player Ice Sword(Clone)")
            {
                GameObject newPlayer = Instantiate(brickSword, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                if (facingDirection == (Vector2)(-transform.right))
                {
                    newPlayer.transform.localScale = new Vector2 (-1, 1);
                    newPlayer.GetComponent<Player>().facingDirection = new Vector2(-1, 0);
                }
                Destroy(GameObject.Find("Player Ice Sword(Clone)"));
            }
            if (transform.name == "Player Brick Sword(Clone)")
            {
                GameObject newPlayer = Instantiate(regularSword, new Vector2(transform.position.x, transform.position.y), Quaternion.identity);
                if (facingDirection == (Vector2)(-transform.right))
                {
                    newPlayer.transform.localScale = new Vector2(-1, 1);
                    newPlayer.GetComponent<Player>().facingDirection = new Vector2(-1, 0);
                }
                Destroy(GameObject.Find("Player Brick Sword(Clone)"));
            }
        }
    }
    
    private int SwordId(GameObject sword)
    {
        string name = sword.name;
        if (name.Contains("Flame"))
            return 1;
        if (name.Contains("Brick"))
            return 2;
        if (name.Contains("Ice"))
            return 3;
        if (name.Contains("Light"))
            return 4;
        if (name.Contains("Guitar"))
            return 5;
        return 0;
    }

    private void isGrounded()
    {
        float extraHeightText = 1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0f, Vector2.down, extraHeightText, platformLayerMask);

        grounded = raycastHit.collider != null;
    }
}
