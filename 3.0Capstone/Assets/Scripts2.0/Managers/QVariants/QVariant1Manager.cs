using UnityEngine;

public class QVariant1Manager : MonoBehaviour
{
    
    public void Option1Press()
    {
        Debug.Log("LEFT BUTTON PRESSED");
        GameManager.Instance.AddShards(50);
        GameManager.Instance.Victory();
    }

    public void Option2Press()
    {
        Debug.Log("RIGHT BUTTON PRESSED");
    }

}
