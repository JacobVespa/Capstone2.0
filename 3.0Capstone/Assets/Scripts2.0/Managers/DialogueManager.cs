using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public bool isTalking = false;
    public bool hasFinishedTalking = false; // New boolean for when dialogue completes
    
    [SerializeField] private TMP_Text dialogueBox;
    [SerializeField] private float typewriterSpeed = 0.05f;
    [SerializeField] private float dialogueBufferTime = 1.0f; // Time to wait after dialogue finishes
    
    // Dictionary to store dialogue strings with keys for easy access
    private Dictionary<string, string> dialogues = new Dictionary<string, string>();
    
    private Queue<string> dialogueQueue = new Queue<string>();

    void Start()
    {
        InitializeDialogues();
    }

    private void InitializeDialogues()
    {
        // Placeholder text!
        dialogues.Add("intro", "That last wave took a lot out of us. Get ready for the next wave with this downtime to prepare!");
        dialogues.Add("engine", "The engine is overheating! Press ***** to cool it down.");
        dialogues.Add("smallEnemy", "Enemies incoming! Use your turrets to defend.");
        dialogues.Add("reload", "Your turrets are empty! Bring ammo from the crate and reload!");
        dialogues.Add("repair", "Damage detected! Use repair tools to fix your rig.");
        dialogues.Add("hammer", "Hitch hikers are invading the RIG! Grab your hammer and start swinging!");
        dialogues.Add("largeEnemy", "Watch out! A large enemy is approaching!");
        dialogues.Add("tutorialComplete", "The path ahead is clear!Get ready for the real challenge ahead!");
    }

    // Call this method from TutorialManager to show dialogue
    public void ShowDialogue(string dialogueKey)
    {
        if (dialogues.ContainsKey(dialogueKey))
        {
            if (!isTalking)
            {
                hasFinishedTalking = false; // Reset the flag when starting new dialogue
                StartCoroutine(TypeDialogue(dialogues[dialogueKey]));
            }
            else
            {
                // Queue dialogue if already talking
                dialogueQueue.Enqueue(dialogueKey);
            }
        }
        else
        {
            Debug.LogWarning($"Dialogue key '{dialogueKey}' not found!");
        }
    }

    // Overload to accept direct string
    public void ShowDialogue(string dialogueKey, string customText)
    {
        if (!isTalking)
        {
            hasFinishedTalking = false; // Reset the flag when starting new dialogue
            StartCoroutine(TypeDialogue(customText));
        }
    }

    private IEnumerator TypeDialogue(string dialogue)
    {
        isTalking = true;
        hasFinishedTalking = false;
        dialogueBox.text = "";
        
        foreach (char c in dialogue)
        {
            dialogueBox.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        
        isTalking = false;

        // Wait for buffer time before setting hasFinishedTalking to true
        yield return new WaitForSeconds(dialogueBufferTime);
        hasFinishedTalking = true;

        // Process queued dialogues
        if (dialogueQueue.Count > 0)
        {
            string nextDialogue = dialogueQueue.Dequeue();
            ShowDialogue(nextDialogue);
        }
    }

    // Skip typewriter effect
    public void SkipTypewriter()
    {
        StopAllCoroutines();
        isTalking = false;
        hasFinishedTalking = true;
        dialogueBox.text = "";
    }

    // Clear the dialogue box
    public void ClearDialogue()
    {
        dialogueBox.text = "";
        hasFinishedTalking = false;
    }

    // Reset the finished talking flag (useful for tutorial steps)
    public void ResetFinishedFlag()
    {
        hasFinishedTalking = false;
    }
}