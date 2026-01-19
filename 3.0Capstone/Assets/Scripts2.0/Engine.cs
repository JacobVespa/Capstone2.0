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
    private WallMoving[] wallMove; //Disgusting, forgive me father
    private Color originalColor = Color.white;
    private Color heatColor = Color.red;

    private bool tooHot = false;

    private float heat = 0f;        
    private float targetHeat = 0f;

    private void Start()
    {
       wallMove = FindObjectsByType<WallMoving>(sortMode: FindObjectsSortMode.None);
    }
    // Update is called once per frame
    void Update()
    {
        targetHeat += heatIncreaseRate * Time.deltaTime;
        targetHeat = Mathf.Clamp01(targetHeat);

        heat = Mathf.Lerp(heat, targetHeat, Time.deltaTime * lerpSpeed);

        engineSprite.color = Color.Lerp(originalColor, heatColor, heat);
        EngineBreakdown();
        EngineUpstart();
    }

    public void EngineRepair()
    {
        targetHeat -= repairAmount;
        targetHeat = Mathf.Clamp01(targetHeat);
        Debug.Log("Here");
    }

    private void EngineBreakdown()
    {
        //stop all wall movement
        if (engineSprite.color == heatColor)
        {
            for (int i = 0; i < wallMove.Length; i++)
            {
                wallMove[i].wallMoveSpeed = 0.0f;
                wallMove[i].floorMoveSpeed = 0.0f;
            }
            Debug.Log("HOT!!!!");
        }
    }

    private void EngineUpstart()
    {
        if (engineSprite.color == originalColor)
        {
            for (int i = 0; i < wallMove.Length; i++)
            {
                wallMove[i].wallMoveSpeed = 3.0f;
                wallMove[i].floorMoveSpeed = 2.0f;
            }
            Debug.Log("WORKS!");
        }
    }
}
