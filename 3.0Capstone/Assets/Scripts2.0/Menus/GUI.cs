using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private TMP_Text shardText;
    [SerializeField] private TMP_Text killText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            shardText.text = "X " + GameManager.Instance.Shards;
            killText.text = "X " + GameManager.Instance.Kills;
            //Debug.Log("Shards: " + GameManager.Instance.Shards);
        }
    }

}
