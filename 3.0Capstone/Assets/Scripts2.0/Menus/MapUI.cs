using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [SerializeField] private Slider progressBar;

    private void Update()
    {
        if (GameManager.Instance == null) { return; }
        GetLevelDuration();
        progressBar.value = GameManager.Instance.GameTime;
        if (progressBar.value == progressBar.maxValue) progressBar.enabled = false;
    }

    private void GetLevelDuration()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager == null || gameflowManager.CurrentLevel == null) return;
        
        float levelDurration = gameflowManager.CurrentLevel.Duration;
        if (levelDurration == 0 || progressBar.maxValue == levelDurration) return;

        progressBar.maxValue = levelDurration;
    }
}
