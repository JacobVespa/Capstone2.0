using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class Engine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private float heatIncreaseRate = 0.02f; 
    [SerializeField] private float repairAmount = 0.05f;
    [SerializeField] private float lerpSpeed = 5f;
    private WallMoving wallMove;
    private Color originalColor = Color.white;
    private Color heatColor = Color.red;

    private float heat = 0f;        
    private float targetHeat = 0f;

    private void Start()
    {
        wallMove = FindFirstObjectByType<WallMoving>();
    }
    // Update is called once per frame
    void Update()
    {
        targetHeat += heatIncreaseRate * Time.deltaTime;
        targetHeat = Mathf.Clamp01(targetHeat);

        heat = Mathf.Lerp(heat, targetHeat, Time.deltaTime * lerpSpeed);

        engineSprite.color = Color.Lerp(originalColor, heatColor, heat);
        //EngineBreakdown();
    }

    public void EngineRepair()
    {
        targetHeat -= repairAmount;
        targetHeat = Mathf.Clamp01(targetHeat);
    }

    private void EngineBreakdown()
    {
        //stop all wall movement
        if (engineSprite.color == heatColor)
        {
            wallMove.wallMoveSpeed = 0.0f;
            wallMove.floorMoveSpeed = 0.0f;
            Debug.Log("HOT!!!!");
        }
    }

    private void EngineUpstart()
    {
        if (engineSprite.color == originalColor)
        {
            wallMove.wallMoveSpeed = 3.0f;
            wallMove.floorMoveSpeed = 2.0f;
        }
    }
}
