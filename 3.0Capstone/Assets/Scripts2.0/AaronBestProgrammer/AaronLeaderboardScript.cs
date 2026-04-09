using UnityEngine;
using System.IO;
using System.Collections.Generic;

//using TMPro;
public class AaronLeaderboardScript : MonoBehaviour
{

    LeaderBoardUI boardUI;

    [SerializeField] private bool isMainMenu = false;

    private string scorePath;
    [SerializeField] private List<string> score_JSON;
    [SerializeField] private List<Score> scores = new List<Score>();
    private Score currentScore = new Score();

    public class Score
    {
        public string name;
        public string total;
        public string shards;
        public string kills;
    }
    // {{Name:11}{Total:22}{Shard:33}{Kills:44}}

    private void Start()
    {
        if (isMainMenu) InitilizeBoard("");
    }

    public void InitilizeBoard(string name)
    {
        boardUI = FindFirstObjectByType<LeaderBoardUI>();

        scorePath = Path.Combine(Application.persistentDataPath, "LocalLeaderBoard.txt");
        Debug.Log(scorePath);

        if (!File.Exists(scorePath))
        {
            File.WriteAllText(scorePath, "");
        }

        ReadScore();

        if (!isMainMenu) WriteScore(name);

        ParseScore();

        if (scores.Count > 0)
        {
            currentScore = scores[scores.Count - 1];
        }

        SortBoard();
        DisplayBoard();
    }

    public void WriteScore(string name)
    {
        string newJson = "";

        string shards = "0";
        string kills = "0";
        string total = "0";

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
        string tag = "";

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
                    int start = 0;
                    int end = part.IndexOf('(') + 1;

                    if (end > start)
                    {
                        tag = part.Substring(start, end-1);
                    }

                    start = part.IndexOf('(') + 1;
                    end = part.IndexOf(')');

                    if (start > 0 && end > start)
                    {
                        string result = part.Substring(start, end - start);

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
                bool larger = (float.Parse(scores[j].total)) > (float.Parse(scores[j + 1].total));

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
        int index = 0;

        if (!isMainMenu)
        {
            string currentDisplay = "[" + currentScore.name + "] Total[" + currentScore.total + "] Shards[" + currentScore.shards + "] Kills[" + currentScore.kills + "]";
            boardUI.DisplayScores(currentDisplay);
        }

        boardUI.ResetAfterStart();

        foreach (Score s in scores)
        {
            if (index > 4) break;

            //Debug.Log("Name: " + s.name + " Total: " + s.total + " Shards: " + s.shards + " Kills: " + s.kills);
            //Debug.Log(scores.Count);

            string display = "";

            if (isMainMenu)
            {
                display = "[" + s.name + "] [" + s.total + "]";
            }
            else
            {
                display = "[" + s.name + "] Total[" + s.total + "] Shards[" + s.shards + "] Kills[" + s.kills + "]";
            }

            boardUI.DisplayScores(display);

            index++;
        }
    }
}
