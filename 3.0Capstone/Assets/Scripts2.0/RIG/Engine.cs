using System.Collections;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class Engine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private float heatIncreaseRate = 0.02f; 
    [SerializeField] private float repairAmount = 0.05f;
    [SerializeField] private float lerpSpeed = 5f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] engineRepairClips;

    private WallMoving[] wallMove; //Disgusting, forgive me father
    private Color originalColor = Color.white;
    private Color heatColor = Color.red;

    private const float OVERHEAT_THRESHOLD = 0.99f;
    private const float REPAIR_THRESHOLD = 0.01f;
    private bool tooHot = false;
    

    private float heat = 0f;        
    private float targetHeat = 0f;

    [SerializeField] public GameObject buttonPromptXB;

    private void Start()
    {
       buttonPromptXB.SetActive(false);
       wallMove = FindObjectsByType<WallMoving>(sortMode: FindObjectsSortMode.None);
    }
    // Update is called once per frame
    void Update()
    {
        targetHeat += heatIncreaseRate * Time.deltaTime;
        targetHeat = Mathf.Clamp01(targetHeat);

        heat = Mathf.Lerp(heat, targetHeat, Time.deltaTime * lerpSpeed);

        engineSprite.color = Color.Lerp(originalColor, heatColor, heat);
        //Debug.Log("Heat: " + heat);

        EngineBreakdown();
        EngineUpstart();
    }

    public void EngineRepair()
    {
        targetHeat -= repairAmount;
        targetHeat = Mathf.Clamp01(targetHeat);
        EngineRepairSFX();
        //Debug.Log("Here");
    }

    //Might be a better way to do this
    public void EngineRepairSFX()
    {
        int clipIndex = Random.Range(0, engineRepairClips.Length);
        audioSource.clip = engineRepairClips[clipIndex];
        audioSource.Play();
    }

    private void EngineBreakdown()
    {
        if (!tooHot && heat >= OVERHEAT_THRESHOLD)
        {
            tooHot = true;
            GameManager.Instance.PauseGameTime();

            for (int i = 0; i < wallMove.Length; i++)
            {
                wallMove[i].wallMoveSpeed = 0.0f;
                wallMove[i].floorMoveSpeed = 0.0f;

                GameflowManager flowManager = FindFirstObjectByType<GameflowManager>();
                if (flowManager != null)
                {
                    //flowManager.CurrentLevel.Cinematic.ShakeCamera(1.0f, 0.3f);
                }
            }

            //Debug.Log("ENGINE HOT!!!!");
        }
    }

    private void EngineUpstart()
    {
        if (tooHot && heat <= REPAIR_THRESHOLD)
        {
            tooHot = false;
            GameManager.Instance.StartGameTime();

            for (int i = 0; i < wallMove.Length; i++)
            {
                wallMove[i].wallMoveSpeed = 3.0f;
                wallMove[i].floorMoveSpeed = 2.0f;
            }

            //Debug.Log("ENGINE REPAIRED!");
        }
    }
}
