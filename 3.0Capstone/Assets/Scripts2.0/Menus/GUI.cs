using TMPro;
using UnityEngine;

public class GUI : MonoBehaviour
{
    [SerializeField] private TMP_Text shardText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            shardText.text = "X " + GameManager.Instance.Shards;
            //Debug.Log("Shards: " + GameManager.Instance.Shards);
        }
    }

}
