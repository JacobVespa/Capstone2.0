using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class MMScoreboard : MonoBehaviour
{

    [SerializeField] private List<string> scores;
    [SerializeField] private List<TMP_Text> textBoxes;

    private string mainPath;
    private int index = 0;

    private int scoreLimit = 10;

    void Start()
    {
        mainPath = Path.Combine(Application.persistentDataPath, "LocalLeaderBoard.txt");

        if (File.Exists(mainPath))
        {

        }
    }
    public void ReadScore()
    {
        if (File.Exists(mainPath))
        {
            using (StreamReader sr = new StreamReader(mainPath))
            {
                while (!sr.EndOfStream && index < scoreLimit)
                {
                    scores.Add(sr.ReadLine());
                    index++;
                }
            }
        }
    }
}
