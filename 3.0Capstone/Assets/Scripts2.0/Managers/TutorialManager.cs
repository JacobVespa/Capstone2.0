using System;
using System.Collections;
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
    private bool gameStartFlag = false;

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
                turret.UpdateAmmoUI();
            }
        }
        else 
        {
            Debug.LogError("RIG not found in the scene.");
        }
    }

    void Update()
    {
        if (tutorialSteps.Count > 0 && tutorialSteps[0].Invoke())
        {
            tutorialSteps.RemoveAt(0);
        }
        else if (tutorialSteps.Count == 0 && !gameStartFlag)
        {
            gameStartFlag = true;
            StartGame();
        }

        if (RIG != null && rigHealthScript.CanTakeDamage)
        {
            if (rigHealthScript.HealthNormalized <= 0.6f)
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
            smallEnemySpawner.StartNewWave();
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
            largeEnemySpawner.StartNewWave();
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
                turret.UpdateAmmoUI();
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
            while (rigHealthScript.HealthNormalized >= 0.6f)
            {
                rigHealthScript.ApplyDamage(rigHealthScript.DamageThreshold); // Reduce health to trigger repair tutorial
            }
        }

        if (RIG != null && rigHealthScript.HealthNormalized >= 0.8f)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialHammer()
    {
        if (!hammerFlag) // One time trigger for tutorial spawns and actions
        {
            hammerFlag = true;
            dialogueManager.ShowDialogue("hammer");
            tickEnemySpawner.StartNewWave();
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

#region Helper Methods

    private void StartGame()
    {
        StartCoroutine(StartGame(1f)); // Delay to allow tutorial completion dialogue to finish
    }

    private IEnumerator StartGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        //GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.StartWindDownLevel(true);
        }
        else
        {
            Debug.LogError("LevelManager not found in the scene.");
        }
    }

#endregion

}
