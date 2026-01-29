using UnityEngine;
using System;

public class Level
{
    protected LevelManager levelManager => LevelManager.Instance;

    public enum LevelType
    {
        SCROLLER = 1,
        DEFENCE = 2,
        BOSS = 3
    }

    protected LevelType levelType;
    protected int sceneIndex;
    protected float duration;
    protected int finalWave;
    protected int waves;

    public float Duration => duration;
    public int FinalWave => finalWave;

    public Level(LevelType type, int index, float durationSeconds)
    {
        this.levelType = type;
        this.sceneIndex = index;
        this.duration = durationSeconds;
    }

    public Level(LevelType type, int index, int TotalWaves)
    {
        this.levelType = type;
        this.sceneIndex = index;
        this.finalWave = TotalWaves;
    }

    public virtual void StartLevel()
    {
        if (levelManager == null) return;

        levelManager.LoadScene(sceneIndex, OnSceneLoaded);
    }

    // This is called automatically when the scene finishes loading
    protected virtual void OnSceneLoaded()
    {
        MonoStart();
    }

    // Override this in child classes for MonoBehaviour-like initialization
    public virtual void MonoStart()
    {
        // Base implementation - override in child classes
    }

    public virtual void WindDownLevel(bool goNext)
    {
        if (levelManager == null) return;

        levelManager.StartWindDownLevel(goNext);
    }

    public virtual void EndLevel()
    {
        if (levelManager == null) return;

        levelManager.LoadScene(0);
        SoundManager.Instance.PlayBGM("CaveFight");
    }

    public virtual void RestartLevel()
    {
        if (levelManager == null) return;

        levelManager.LoadScene(sceneIndex, OnSceneLoaded);
    }

    public LevelType GetLevelType()
    {
        return levelType;
    }
}