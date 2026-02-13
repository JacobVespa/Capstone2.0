using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private List<Func<bool>> tutorialSteps;

    private bool introFlag = false;
    private bool engineFlag = false;
    private bool smallEnemyFlag = false;
    private bool largeEnemyFlag = false;
    private bool reloadFlag = false;
    private bool repairFlag = false;
    private bool hammerFlag = false;

    [SerializeField] private WaveSpawner smallEnemySpawner;
    [SerializeField] private WaveSpawner largeEnemySpawner;
    private GameObject RIG;
    private Engine engineScript;

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
            TutorialEnemyLarge
        };

        RIG = GameObject.FindGameObjectWithTag("Rig");

        if (RIG != null) engineScript = RIG.GetComponent<Engine>();
        else Debug.LogError("RIG not found in the scene.");


    }

    void Update()
    {
        if (tutorialSteps[0].Invoke())
        {
            tutorialSteps.RemoveAt(0);
        }
    }

#region Tutorial Step Methods

    private bool TutorialIntroduction()
    {
        if (!introFlag) // One time trigger for tutorial spawns and actions
        {
            introFlag = true;
        }
        return false;
    }

    private bool TutorialEngine()
    {
        if (!engineFlag) // One time trigger for tutorial spawns and actions
        {
            engineScript.IsOverheated(true);
            engineFlag = true;
        }

        if(!engineScript.TooHot)
        {
            return true;
        }

        return false;
    }

    private bool TutorialEnemySmall()
    {
        if (!smallEnemyFlag) // One time trigger for tutorial spawns and actions
        {
            smallEnemyFlag = true;
        }

        return false;
    }

    private bool TutorialEnemyLarge()
    {
        if (!largeEnemyFlag) // One time trigger for tutorial spawns and actions
        {
            largeEnemyFlag = true;
        }

        return false;
    }

    private bool TutorialReload()
    {
        if (!reloadFlag) // One time trigger for tutorial spawns and actions
        {
            reloadFlag = true;
        }
        return false;
    }

    private bool TutorialRepair()
    {
        if (!repairFlag) // One time trigger for tutorial spawns and actions
        {
            repairFlag = true;
        }

        return false;
    }

    private bool TutorialHammer()
    {
        if (!hammerFlag) // One time trigger for tutorial spawns and actions
        {
            hammerFlag = true;
        }

        return false;
    }

#endregion
}
