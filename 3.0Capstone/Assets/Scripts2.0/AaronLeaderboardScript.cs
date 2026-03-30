using UnityEngine;
using System.IO;
//using TMPro;
public class AaronLeaderboardScript : MonoBehaviour
{
    void CreateLeaderboard()
    {
        string path = Application.dataPath + "/LocalLeaderboard.txt";

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Highscores \n\n");
        }

        string content = "best score wow";

        File.AppendAllText(path, content);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateLeaderboard();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
