using System.Collections;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    private int charTalking;
    public bool isTalking = false;
    public string currentDialogue;

    [SerializeField]
    private TMP_Text dialogueBox;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentDialogue = "lol lmao";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.R) && !isTalking)
        {
            StartCoroutine(Speak());
        }
    }

    IEnumerator Speak()
    {
        isTalking = true;
        dialogueBox.text = "";
        foreach (char c in currentDialogue)
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(0.1f);
        }
        isTalking = false;
    }

    //Go ahead and grab some of those bug repellant pellets.
    //They are totally non-harmful and just scare off the bugs.
}
