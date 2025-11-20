using System.Diagnostics.Contracts;
using UnityEngine;

public class PleaseHelp : MonoBehaviour
{

    public GameObject drillInteractor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drillInteractor.GetComponent<Station>().enabled = true;
    }

}
