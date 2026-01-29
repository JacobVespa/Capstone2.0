using UnityEngine;

public class LevelSelectDebug : MonoBehaviour
{
    public void StartScrollerLevel()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartScrollerLevel();
        }
    }

    public void StartDefenseLevel()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartDefenseLevel();
        }
    }
}
