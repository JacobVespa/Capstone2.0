using UnityEngine;

public class DevShortcuts : MonoBehaviour
{
    private void Update()
    {
        GameflowManager flowManager = FindFirstObjectByType<GameflowManager>();

        if (flowManager != null)
        {
            AutoFinishLevel(flowManager);
            StartSpecificLevel(flowManager);
            BackToMainMenu(flowManager);
        }
    }

    //Finish Level
    private void AutoFinishLevel(GameflowManager flow)
    {
        if (!Input.GetKeyUp(KeyCode.Alpha0)) return;

        if (flow.LevelRunning == true)
        {
            if (flow.CurrentLevel is DefenceLevel defence)
            {
                defence.WavesCompleted = 10;
            }
            else
            {
                GameManager.Instance.GameTime = 999999f;
            }
        }
    }

    //Start Level 1-4
    private void StartSpecificLevel(GameflowManager flow)
    {
        if (flow.LevelRunning) return;

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            flow.StartScrollerLevel();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            flow.StartDefenseLevel();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            flow.StartGoldRushLevel();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            flow.Randomize();
        }
    }

    //Back to Main Restart Everything
    private void BackToMainMenu(GameflowManager flow)
    {

    }
}
