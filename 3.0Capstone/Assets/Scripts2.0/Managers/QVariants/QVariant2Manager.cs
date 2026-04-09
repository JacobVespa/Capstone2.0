using System.Collections;
using TMPro;
using UnityEngine;

public class QVariant2Manager : MonoBehaviour
{
    private bool buttonPressed = false;
    private AudioSource audioSource;
    [SerializeField] private ParticleSystem crystalShards;
    [SerializeField] TMP_Text dialogue;

    //random number for choosing fate
    private int randInt;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }


    public void ButtonPress()
    {
        if (buttonPressed) return;

        buttonPressed = true;
        randInt = Random.Range(0, 2);
        GameManager.Instance.cameFromQVariant2 = true;
        GameflowManager flow = FindFirstObjectByType<GameflowManager>();

        if(randInt == 0)
        {
            StartCoroutine(BadResult(flow));
        }
        else
        {
            StartCoroutine(GoodResult(flow));
        }
    }

    private IEnumerator GoodResult(GameflowManager flow) //rand int is 1
    {
        audioSource.Play();
        crystalShards.Play();
        GameManager.Instance.AddShards(50);

        //good part
        dialogue.text = "Nice! We got the shards safely, now lets get out of here";
        yield return new WaitForSeconds(1.5f);
        flow.LevelRunning = false;
        GameManager.Instance.Victory();
    }

    private IEnumerator BadResult(GameflowManager flow) //rand int is 0
    {
        audioSource.Play();
        crystalShards.Play();
        GameManager.Instance.AddShards(50);

        //bad part
        dialogue.text = "oh no! We're getting ambushed! lets make a run for it";
        yield return new WaitForSeconds(3);
        flow.LevelRunning = false;
        flow.StartScrollerLevel();
    }

}
