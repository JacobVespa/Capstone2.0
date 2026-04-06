using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text[] scoreTexts;
    int index = 5;

    public void DisplayScores(string display)
    {
        if (index > 5) return;

        scoreTexts[index].text = display;
        index++;
    }

    public void ResetAfterStart()
    {
        index = 0;
    }
}
