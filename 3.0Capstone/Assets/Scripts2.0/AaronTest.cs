using System.Collections;
using UnityEngine;

public class AaronTest : MonoBehaviour
{
    [SerializeField] private GameObject sect;
    [SerializeField] private int maxSect = 20;
    private Vector2 originPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originPos = transform.position;
        //for (int i = 0; i < maxSect; i++)
        //{
        //    Instantiate(sect.transform, originPos - new Vector2(0, (i+1)*3), Quaternion.identity);
        //}

        StartCoroutine(GenerateSect());

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GenerateSect()
    {
        for (int i = 0; i < maxSect; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Instantiate(sect.transform, originPos, Quaternion.identity);
        }

        yield return null;
    }
}
