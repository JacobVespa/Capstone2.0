using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<Func<bool>> tutorialSteps;
    [SerializeField] private DialogueManager dialogueManager;

    // boolean tutorial flags
    private bool introFlag = false;
    private bool engineFlag = false;
    private bool crystalFlag = false;
    private bool smallEnemyFlag = false;
    private bool largeEnemyFlag = false;
    private bool repairFlag = false;
    private bool hammerFlag = false;
    private bool tutorialCompleteFlag = false;
    private bool gameStartFlag = false;

    // boolean start/finish flags
    private bool hasStarted = false;
    private bool hasFinished = false;

    [SerializeField] private WaveSpawner smallEnemySpawner;
    [SerializeField] private WaveSpawner largeEnemySpawner;
    [SerializeField] private WaveSpawner tickEnemySpawner;
    private GameObject RIG;
    private Engine engineScript;
    private Turret[] turretScripts;
    private RigHealth rigHealthScript;
    private int oldShardCount;

    //Cinematic Camera
    [SerializeField] private CameraCinematic cinematicCamera;

    void Start()
    {
        if (cinematicCamera == null)
        {
            Debug.LogError("CameraCinematic not found in the scene.");
        }

        tutorialSteps = new List<Func<bool>>
        {
            TutorialIntroduction,
            TutorialEngine,
            TutorialCrystal,
            TutorialEnemySmall,
            TutorialHammer,
            TutorialEnemyLarge,
            TutorialRepair,
            TutorialComplete
        };

        RIG = GameObject.FindGameObjectWithTag("Rig");

        if (RIG != null)
        {
            engineScript = RIG.GetComponentInChildren<Engine>();
            turretScripts = RIG.GetComponentsInChildren<Turret>();
            rigHealthScript = RIG.GetComponent<RigHealth>();
        }
        else 
        {
            Debug.LogError("RIG not found in the scene.");
        }

        EnterTutorial();
    }

    void Update()
    {
        if (hasStarted)
        {
            if (tutorialSteps.Count > 0 && tutorialSteps[0].Invoke())
            {
                tutorialSteps.RemoveAt(0);
            }
            else if (tutorialSteps.Count == 0 && !gameStartFlag && !hasFinished)
            {
                ExitTutorial();
            }
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
        }

        if(!engineScript.TooHot)
        {
            dialogueManager.SkipTypewriter();
            return true;
        }

        return false;
    }

    private bool TutorialCrystal()
    {
        if (!crystalFlag) // One time trigger for tutorial spawns and actions
        {
            crystalFlag = true;
            dialogueManager.ShowDialogue("crystal");
            oldShardCount = GameManager.Instance.Shards; // Store initial shard count to detect changes
        }

        if (GameManager.Instance != null && GameManager.Instance.Shards >= oldShardCount + 4)
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

    //private bool TutorialReload()
    //{
    //    if (!reloadFlag) // One time trigger for tutorial spawns and actions
    //    {
    //        reloadFlag = true;
    //        dialogueManager.ShowDialogue("reload");
    //
    //        foreach (var turret in turretScripts)
    //        {
    //            turret.currentAmmo = 0;
    //            turret.UpdateAmmoUI();
    //            turret.needsReload = true;
    //        }
    //    }
    //
    //    foreach (var turret in turretScripts)
    //    {
    //        if (!turret.needsReload)
    //        {
    //            dialogueManager.SkipTypewriter();
    //            return true;
    //        }
    //    }
    //
    //    return false;
    //}

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

    private void EnterTutorial()
    {
        StartCoroutine(EnterTutorial(3f));
    }

    private void ExitTutorial()
    {
        StartCoroutine(ExitTutorial(3f));
    }

    private IEnumerator StartGame(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.WindDownLevel(true); // Transition to main game after tutorial
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }

    private IEnumerator EnterTutorial(float delay)
    {
        engineScript.Heat = 1f; // Start with overheated engine to trigger tutorial

        if (cinematicCamera != null)
            cinematicCamera.MoveCameraTo(new Vector3(0f, 0f, 0f), delay);

        yield return new WaitForSeconds(delay);

        hasStarted = true;
    }

    private IEnumerator ExitTutorial(float delay)
    {
        yield return new WaitForSeconds(delay);

        gameStartFlag = true;
        StartGame();

        hasFinished = true;
    }

#endregion

}
