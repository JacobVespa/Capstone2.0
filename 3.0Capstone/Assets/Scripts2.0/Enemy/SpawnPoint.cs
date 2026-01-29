using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private List<GameObject> targetPoints = new List<GameObject>();

    public GameObject returnTargetPoint()
    {
        return targetPoints[Random.Range(0,targetPoints.Count)];
    }
}
