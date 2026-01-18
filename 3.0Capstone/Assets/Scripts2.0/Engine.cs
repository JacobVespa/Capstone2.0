using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class Engine : MonoBehaviour
{
    [SerializeField] private SpriteRenderer engineSprite;
    [SerializeField] private WallMoving wallMove;
    private Color originalColor;
    private Color heatColor;

    private void Start()
    {
        originalColor = Color.white;
        heatColor = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }
    // Update is called once per frame
    void Update()
    {
        EngineOverheat();
    }

    private void EngineOverheat()
    {
        StartCoroutine(LerpColor(originalColor, heatColor, 100.0f));
    }

    public void EngineRepair()
    {
        Color currentColor = engineSprite.color;
        Color newColor = new Color(currentColor.r -= 0.05f, 0.0f, 0.0f);

        StartCoroutine(LerpColor(currentColor, newColor, 0.5f));
    }

    private void EngineBreakdown()
    {
        //stop all wall movement
        
    }

    private IEnumerator LerpColor(Color startingColor, Color endingColor, float time)
    {
        float inversedTime = 1 / time; // Compute this value **once**
        for (float step = 0.0f; step < 1.0f; step += Time.deltaTime * inversedTime)
        {
            engineSprite.color = Color.Lerp(startingColor, endingColor, step);
 
            yield return null;
        }
    }


}
