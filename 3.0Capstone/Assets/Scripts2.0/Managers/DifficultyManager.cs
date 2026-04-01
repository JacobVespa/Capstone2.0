using System;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    // Option 1, more bugs
    // Option 2, bugs have more health
    // Option 3, bugs have more movement speed

    //-Difficulty equation: stages cleared () caves cleared () gems collected
    // Stages cleared is a slow ramp up
    // Caves cleared is a medium ramp up
    // Gems collected is fast but temporary ramp up

    //-What needs to be accessed
    // [Enemy] health, movement rate/speed
    // [Spawner] amounts per wave (should not make defense levels too long)

    public static DifficultyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitiateStats();
    }

    [Header("Initial Grub Stats")]
    [SerializeField] private float InitialGrub_Health;
    [SerializeField] private float InitialGrub_Speed;

    [Header("Initial Skeeto Stats")]
    [SerializeField] private float InitialSkeeto_Health;
    [SerializeField] private float InitialSkeeto_Speed;

    [Header("Initial BottomFeeder Stats")]
    [SerializeField] private float InitialBottomFeeder_Health;
    [SerializeField] private float InitialBottomFeeder_Speed;

    [Header("Initial Tick Stats")]
    [SerializeField] private float InitialTick_Health;
    [SerializeField] private float InitialTick_Speed;

    [Header("Inital Runner Stats")]
    [SerializeField] private float InitialRunner_Health;
    [SerializeField] private float InitialRunner_Speed;

    [Header("Stat Multipliers")]
    [SerializeField] private float stageClear_M = 0.05f;
    [SerializeField] private float caveClear_M = 0.08f;
    [SerializeField] private float gemCollected_M = 0.1f;

    // Progress variables
    [Header("Progress Variables")]
    [SerializeField] private float stagesCleared = -1;
    [SerializeField] private float cavesCleared = 0;
    [SerializeField] private float gemsCollected = 0;

    // Dynamic variables -------------------------------------------------
    private float currentGrub_Health;
    public float GrubHealth => currentGrub_Health;
    private float currentGrub_Speed;
    public float GrubSpeed => currentGrub_Speed;
    // Skeeto -------------------------------------------------
    private float currentSkeeto_Health;
    public float SkeetoHealth => currentSkeeto_Health;
    private float currentSkeeto_Speed;
    public float SkeetoSpeed => currentSkeeto_Speed;
    // BottomFeeder -------------------------------------------------
    private float currentBottomFeeder_Health;
    public float BottomFeederHealth => currentBottomFeeder_Health;
    private float currentBottomFeeder_Speed;
    public float BottomFeederSpeed => currentBottomFeeder_Speed;
    // Tick -------------------------------------------------
    private float currentTick_Health;
    public float TickHealth => currentTick_Health;
    private float currentTick_Speed;
    public float TickSpeed => currentTick_Speed;
    // Big Tick -------------------------------------------------
    private float currentRunner_Health;
    public float RunnerHealth => currentRunner_Health;
    private float currentRunner_Speed;
    public float RunnerSpeed => currentRunner_Speed;
    // -------------------------------------------------

    private void Start()
    {
        InitiateStats();
    }

    public void ResetDifficulty()
    {
        stagesCleared = -1;
        cavesCleared = 0;
        gemsCollected = 0;

        InitiateStats();
    }

    public void StageCleared()
    {
        stagesCleared++;
        UpgradeDifficulty();
    }

    public void CaveCleared()
    {
        cavesCleared++;
        UpgradeDifficulty();
    }

    public void GemCollected()
    {
        gemsCollected++;
        UpgradeDifficulty();
    }

    public void ResetGemsCollected()
    {
        gemsCollected = 0;
        UpgradeDifficulty();
    }


    private void UpgradeDifficulty()
    {
        SetGrub_Stats(InitialGrub_Health * HealthIncrease(), InitialGrub_Speed * SpeedIncrease());
        SetSkeeto_Stats(InitialSkeeto_Health * HealthIncrease(), InitialSkeeto_Speed * SpeedIncrease());
        SetBottomFeeder_Stats(InitialBottomFeeder_Health * HealthIncrease(), InitialBottomFeeder_Speed * SpeedIncrease());
        SetTick_Stats(InitialTick_Health * HealthIncrease(), InitialTick_Speed * SpeedIncrease());
        SetRunner_Stats(InitialRunner_Health * HealthIncrease(), InitialRunner_Speed * SpeedIncrease());
    }

    private float HealthIncrease()
    {
        return 1 + (stagesCleared * stageClear_M) + (cavesCleared * caveClear_M) + (gemsCollected * gemCollected_M);
    }

    private float SpeedIncrease()
    {
        return 1 + (stagesCleared * stageClear_M) + (cavesCleared * caveClear_M) + (gemsCollected * gemCollected_M);
    }

    private void SetGrub_Stats(float health, float speed)
    {
        currentGrub_Health = health;
        currentGrub_Speed = speed;
    }

    private void SetSkeeto_Stats(float health, float speed)
    {
        currentSkeeto_Health = health;
        currentSkeeto_Speed = speed;
    }

    private void SetBottomFeeder_Stats(float health, float speed)
    {
        currentBottomFeeder_Health = health;
        currentBottomFeeder_Speed = speed;
    }

    private void SetTick_Stats(float health, float speed)
    {
        currentTick_Health = health;
        currentTick_Speed = speed;
    }

    private void SetRunner_Stats(float health, float speed)
    {
        currentRunner_Health = health;
        currentRunner_Speed = speed;
    }

    private void InitiateStats()
    {
        // Grub
        currentGrub_Health = InitialGrub_Health;
        currentGrub_Speed = InitialGrub_Speed;
        // Skeeto
        currentSkeeto_Health = InitialSkeeto_Health;
        currentSkeeto_Speed = InitialSkeeto_Speed;
        // BottomFeeder
        currentBottomFeeder_Health = InitialBottomFeeder_Health;
        currentBottomFeeder_Speed = InitialBottomFeeder_Speed;
        // Tick
        currentTick_Health = InitialTick_Health;
        currentTick_Speed = InitialTick_Speed;
        // Big Tick 
        currentRunner_Health = InitialRunner_Health;
        currentRunner_Speed = InitialRunner_Speed;
    }

}
