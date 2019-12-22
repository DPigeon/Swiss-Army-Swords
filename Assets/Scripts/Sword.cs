using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Sword : MonoBehaviour
{
    [SerializeField] int damageDealt = 1;
    // How quickly enemy takes damage
    protected Animator swordAnimator;
    protected BoxCollider2D swordCollider;
    SwordType _swordType;

    [SerializeField]
    protected float damageDuration = 0.2f;
    protected float damageDelay;
    protected Player player;
    protected SwordInventory inventory;
    protected bool isAbilityUsed;
    public bool damaging;
    int _damage = 1;

	public Button mobileAttackButton;
    public float cooldownTimer = 2;
    
    AudioSource audioSource;
    int randomAudioIndex = -1;
    int previousAudioIndex = -1;
    GameObject pauseMenu;
    public Scene scene;
    public bool dialogueActive;

    public enum SwordType
    {
        REGULAR, ICE, BRICK, LIGHT, FIRE
    }

    public SwordType swordType { get { return _swordType; } set { _swordType = value; } }

    public int damage { get { return _damage; } set { _damage = value; } }

    public virtual void Start()
    {
        damageDelay = damageDuration * damageDealt;
        damaging = false;
        swordAnimator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        inventory = GameObject.Find("InventoryManager").GetComponent<SwordInventory>();
		mobileAttackButton = GameObject.Find("AttackButton").GetComponent<Button>();
		mobileAttackButton.onClick.AddListener(SlashSword);
        swordCollider = player.GetComponentInChildren<BoxCollider2D>();
        swordCollider.enabled = false;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.2f;
        pauseMenu = GameObject.Find("PauseMenu");
    }

    public virtual void Update()
    {
        if (swordType != SwordType.LIGHT || swordType != SwordType.FIRE)
        {
            cooldownTimer -= Time.deltaTime;
        }

        scene = SceneManager.GetActiveScene();

        if (scene.name == "Cutscene" || scene.name == "FinalCutscene" || !inventory.switchSwords || dialogueActive) // prevent input during cutscenes or pickup
        { }
        else
        { 
            if (Input.GetButtonDown("Fire1") && damaging == false && !pauseMenu.GetComponent<Pause>().paused)
			{
				Attack();
				swordCollider.enabled = true;
				makeAttackSound();
			}

            if (damaging)
            {
                damageDelay -= Time.deltaTime;

                if (damageDelay <= 0f)
                {
                    damaging = false;
                    swordCollider.enabled = false;
                }
            }
            if (Input.GetButtonDown("Fire2") && swordType == SwordType.LIGHT)
            {
                Ability();
            }
            else if (cooldownTimer <= 0)
            {
                if (Input.GetButtonDown("Fire2") && swordType != SwordType.FIRE)
                {
                    Ability();
                    cooldownTimer = 2;
                }
            }
        }
    }

    public void makeAttackSound()
    {
        //guarantee to never repeat a sound with this loop
        do
        {
            randomAudioIndex = UnityEngine.Random.Range(0, player.attacksSounds.Length);
        } while (randomAudioIndex == previousAudioIndex);
        audioSource.clip = player.attacksSounds[randomAudioIndex];
        audioSource.time = 0.1f;
        audioSource.Play();
        previousAudioIndex = randomAudioIndex;
    }
	
	public void SlashSword() { // For mobile
		if (damaging == false && !pauseMenu.GetComponent<Pause>().paused)
		{
			Debug.Log("hi");
			Attack();
			swordCollider.enabled = true;
			makeAttackSound();
		}
	}

    public virtual void Attack()
    {
        damaging = true;
        swordAnimator.SetTrigger("attack");
        damageDelay = damageDuration * damageDealt;
    }

    public virtual void Ability()
    {
        swordAnimator.SetTrigger("ability");
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {

    }
}
