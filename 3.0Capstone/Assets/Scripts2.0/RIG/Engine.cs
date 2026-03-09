using UnityEngine;

public class Engine : MonoBehaviour
{
    //[SerializeField] private SpriteRenderer engineSprite; WILL PROB NEED AGAIN LATER, maybe...
    [SerializeField] private float heatIncreaseRate = 0.02f; 
    [SerializeField] private float repairAmount = 0.05f;
    [SerializeField] private float lerpSpeed = 5f;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] engineRepairClips;

    [SerializeField] private GameObject[] damageStages;

    private Color originalColor = Color.white;
    private Color heatColor = Color.red;

    private const float OVERHEAT_THRESHOLD = 0.99f;
    private const float REPAIR_THRESHOLD = 0.01f;

    private bool tooHot = false;
    public bool TooHot => tooHot;

    private float heat = 0f;        

    [SerializeField] public GameObject buttonPromptXB;

    private void Start()
    {
        buttonPromptXB.SetActive(false);

        if (damageStages.Length < 4)
        {
            Debug.Log("A engine damage state sprite might be missing");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Heat only increases if the engine is not overheated
        if (!tooHot)
        {
            heat += heatIncreaseRate * Time.deltaTime;
        }

        heat = Mathf.Clamp01(heat);

        //damageStages[0].color = Color.Lerp(originalColor, heatColor, heat); //AGAIN WILL NEED THIS LATER
        damageStages[0].GetComponent<SpriteRenderer>().color = Color.Lerp(originalColor, heatColor, heat);

        //Debug.Log("Heat: " + heat);

        EngineBreakdown();
        EngineUpstart();
    }

    public void EngineRepair()
    {
        // Only allow repair when overheated
        if (!tooHot) return;

        heat -= repairAmount;
        heat = Mathf.Clamp01(heat);

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
            IsOverheated(true);

            if(GameManager.Instance != null)
            {
                GameManager.Instance.PauseGameTime();
            }

            WallMoving[] walls = GameObject.FindObjectsByType<WallMoving>(FindObjectsSortMode.None);
            foreach (WallMoving wall in walls)            
            {
                wall.Pause();
                GameManager.Instance.ToggleStorageMovement(false);
            }

            //Debug.Log("ENGINE HOT!!!!");
        }
    }

    private void EngineUpstart()
    {
        if (tooHot && heat <= REPAIR_THRESHOLD)
        {
            IsOverheated(false);

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGameTime();
            }

            WallMoving[] walls = GameObject.FindObjectsByType<WallMoving>(FindObjectsSortMode.None);
            foreach (WallMoving wall in walls)            
            {
                wall.Play();
                GameManager.Instance.ToggleStorageMovement(true);
            }

            //Debug.Log("ENGINE REPAIRED!");
        }
    }

    public void IsOverheated(bool value)
    {
        tooHot = value;
    }
}