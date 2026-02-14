using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<Func<bool>> tutorialSteps;
    [SerializeField] private DialogueManager dialogueManager;

    private bool introFlag = false;
    private bool engineFlag = false;
    private bool smallEnemyFlag = false;
    private bool largeEnemyFlag = false;
    private bool reloadFlag = false;
    private bool repairFlag = false;
    private bool hammerFlag = false;
    private bool tutorialCompleteFlag = false;

    [SerializeField] private WaveSpawner smallEnemySpawner;
    [SerializeField] private WaveSpawner largeEnemySpawner;
    [SerializeField] private WaveSpawner tickEnemySpawner;
    private GameObject RIG;
    private Engine engineScript;
    private Turret[] turretScripts;
    private RigHealth rigHealthScript;

    void Start()
    {
        tutorialSteps = new List<Func<bool>>
        {
            TutorialIntroduction,
            TutorialEngine,
            TutorialEnemySmall,
            TutorialReload,
            TutorialRepair,
            TutorialHammer,
            TutorialEnemyLarge,
            TutorialComplete
        };

        RIG = GameObject.FindGameObjectWithTag("Rig");

        if (RIG != null)
        {
            engineScript = RIG.GetComponentInChildren<Engine>();
            turretScripts = RIG.GetComponentsInChildren<Turret>();
            rigHealthScript = RIG.GetComponent<RigHealth>();

            foreach (var turret in turretScripts)
            {
                turret.currentAmmo = 999;
            }
        }
        else 
        {
            Debug.LogError("RIG not found in the scene.");
        }
    }

    void Update()
    {
        if (tutorialSteps[0].Invoke())
        {
            tutorialSteps.RemoveAt(0);
        }

        if (RIG != null && rigHealthScript.CanTakeDamage)
        {
            if (rigHealthScript.HealthNormalized <= 0.8f)
            {
                rigHealthScript.CanTakeDamage = false; // Prevent further damage during tutorial
            }
        }
    }

#region Tutorial Step Methods

    private bool TutorialIntroduction()
    {
        if (!introFlag) // One time trigger for tutorial spawns and actions
        {
            introFlag = true;
            dialogueManager.ShowDialogue("intro");
        }

        if (dialogueManager.hasFinishedTalking)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialEngine()
    {
        if (!engineFlag) // One time trigger for tutorial spawns and actions
        {
            engineFlag = true;
            dialogueManager.ShowDialogue("engine");
            engineScript.IsOverheated(true);
        }

        if(!engineScript.TooHot)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialEnemySmall()
    {
        if (!smallEnemyFlag) // One time trigger for tutorial spawns and actions
        {
            smallEnemyFlag = true;
            dialogueManager.ShowDialogue("smallEnemy");
        }

        if (smallEnemySpawner != null && smallEnemySpawner.IsWaveComplete)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialEnemyLarge()
    {
        if (!largeEnemyFlag) // One time trigger for tutorial spawns and actions
        {
            largeEnemyFlag = true;
            dialogueManager.ShowDialogue("largeEnemy");
        }

        if (largeEnemySpawner != null && largeEnemySpawner.IsWaveComplete)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialReload()
    {
        if (!reloadFlag) // One time trigger for tutorial spawns and actions
        {
            reloadFlag = true;
            dialogueManager.ShowDialogue("reload");

            foreach (var turret in turretScripts)
            {
                turret.currentAmmo = 0;
                turret.needsReload = true;
            }
        }

        foreach (var turret in turretScripts)
        {
            if (!turret.needsReload)
            {
                dialogueManager.SkipTypewriter();
                return true;
            }
        }

        return false;
    }

    private bool TutorialRepair()
    {
        if (!repairFlag) // One time trigger for tutorial spawns and actions
        {
            repairFlag = true;
            dialogueManager.ShowDialogue("repair");
        }

        return false;
    }

    private bool TutorialHammer()
    {
        if (!hammerFlag) // One time trigger for tutorial spawns and actions
        {
            hammerFlag = true;
            dialogueManager.ShowDialogue("hammer");
        }

        if (tickEnemySpawner != null && tickEnemySpawner.IsWaveComplete)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialComplete()
    {
        if (!tutorialCompleteFlag) // One time trigger for tutorial spawns and actions
        {
            tutorialCompleteFlag = true;
            dialogueManager.ShowDialogue("tutorialComplete");
            rigHealthScript.CanTakeDamage = true; // Allow damage to RIG after tutorial is complete
        }

        if (dialogueManager.hasFinishedTalking)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

#endregion
}
