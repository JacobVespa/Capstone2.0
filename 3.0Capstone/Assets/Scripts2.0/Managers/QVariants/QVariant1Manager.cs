using UnityEngine;

public class QVariant1Manager : MonoBehaviour
{
    private bool buttonPressed = false;
    
    public void Option1Press()
    {
        if (buttonPressed) return;

        Debug.Log("LEFT BUTTON PRESSED");
        buttonPressed = true;
        GameManager.Instance.AddShards(50);
        GameflowManager flow = FindFirstObjectByType<GameflowManager>();
        flow.LevelRunning = false;
        GameManager.Instance.Victory();
    }

    public void Option2Press()
    {
        if (buttonPressed) return;

        buttonPressed = true;
        Debug.Log("RIGHT BUTTON PRESSED");
    }

}
