using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
//using TMPro;
public class AaronLeaderboardScript : MonoBehaviour
{

    private string scorePath;
    [SerializeField] private List<string> score_JSON;
    [SerializeField] private List<Score> scores;

    public class Score
    {
        public string name;
        public string total;
        public string shards;
        public string kills;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scorePath = Path.Combine(Application.persistentDataPath, "LocalLeaderBoard.txt");
        Debug.Log(scorePath);

        InitilizeBoard();
    }
    // {{Name:11}{Total:22}{Shard:33}{Kills:44}}

    private void InitilizeBoard()
    {
        if (!File.Exists(scorePath))
        {
            File.WriteAllText(scorePath, "");
        }

        ReadScore();
        WriteScore("Test1!");
        WriteScore("Test2!");
        WriteScore("Test3!");
        ParseScore();
        SortBoard();
    }

    public void WriteScore(string name)
    {
        string newJson = "";

        string shards = "##";
        string kills = "##";
        string total = "##";

        if (GameManager.Instance != null)
        {
            shards = GameManager.Instance.Shards.ToString();
            kills = GameManager.Instance.Kills.ToString();
            total = (GameManager.Instance.Shards + GameManager.Instance.Kills).ToString();
        }

        newJson = "{{Name(" +name+ ")}{Total(" +total+ ")}{Shards(" +shards+ ")}{Kills(" +kills+ ")}}";

        score_JSON.Add(newJson);
        
        using (StreamWriter sw = new StreamWriter(scorePath, true))
        {
            if (newJson != null)
                sw.WriteLine(newJson);
        }

    }

    public void ReadScore()
    {
        if (File.Exists(scorePath))
        {
            using (StreamReader sr = new StreamReader(scorePath))
            {
                while (!sr.EndOfStream)
                {
                    score_JSON.Add(sr.ReadLine());
                }
            }
        }
    }

    private void ParseScore()
    {
        string[] parts;
        string temp;

        for (int i = 0; i < score_JSON.Count; i++)
        {
            Score newScore = new Score();

            if (score_JSON[i].Length > 2)
            {
                temp = score_JSON[i];
                temp = temp.Substring(1, temp.Length - 2);

                parts = temp.Split(new char[] { '{', '}' }, System.StringSplitOptions.RemoveEmptyEntries);

                foreach (string part in parts)
                {
                    string tag = "";

                    int start = part.IndexOf('{') + 1;
                    int end = part.IndexOf('(');

                    if (end > start)
                    {
                        tag = temp.Substring(start, end - start);
                        Debug.Log(tag);
                    }

                    start = part.IndexOf('(') + 1;
                    end = part.IndexOf(')');

                    if (start > 0 && end > start)
                    {
                        string result = temp.Substring(start, end - start);

                        switch(tag)
                        {
                            case "Name":
                                newScore.name = result;
                                break;
                            case "Total":
                                newScore.total = result;
                                break;
                            case "Shards":
                                newScore.shards = result;
                                break;
                            case "Kills":
                                newScore.kills = result;
                                break;
                            default:
                                break;
                        }

                    }
                }
                if (newScore != null) scores.Add(newScore);
            }
        }
    }

    private void SortBoard()
    {
        for (int i = 0; i < scores.Count; i++)
        {
            bool sorted = true;
            for (int j = 0; j < scores.Count - 1; j++)
            {
                Score temp = new Score();
                bool larger = int.Parse((scores[j].total)) > int.Parse((scores[j + 1].total));

                if (!larger)
                {
                    sorted = false;

                    temp = scores[j];
                    scores[j] = scores[j + 1];
                    scores[j + 1] = temp;
                }
            }

            if (sorted) return;
        }
    }

    private void DisplayBoard()
    {
        foreach (Score s in scores)
        {
            Debug.Log(s.name + s.total + s.shards + s.kills);
        }
    }
}
