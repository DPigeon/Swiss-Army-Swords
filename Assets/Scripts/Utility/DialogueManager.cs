using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    public Text dialogueText;
    public Animator animator;
    private Queue<string> sentences;
    Player player;
    Sword sword;
    SwordInventory inventory;

    public static DialogueManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        sentences = new Queue<string>();
        player = GameObject.Find("Player").GetComponent<Player>();
        inventory = GameObject.Find("InventoryManager").GetComponent<SwordInventory>();
        sword = GameObject.FindGameObjectWithTag("Sword").GetComponent<Sword>();
    }

    void Update()
    {
        sword = GameObject.FindGameObjectWithTag("Sword").GetComponent<Sword>();

        if (Input.GetKeyDown("joystick button 0"))
        {
            DisplayNextSentence();
        }
    }

    public void StartDialogue (Dialogue dialogue)
    {
        player.dialogueActive = true;
        sword.dialogueActive = true;
        inventory.dialogueActive = true;
        animator.SetBool("isOpen", true);
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeDialogue(sentence));
    }

    IEnumerator TypeDialogue(string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        player.dialogueActive = false;
        sword.dialogueActive = false;
        inventory.dialogueActive = false;
    }

}
