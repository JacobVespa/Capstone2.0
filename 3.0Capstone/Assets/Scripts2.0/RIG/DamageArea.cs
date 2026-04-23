using System;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    [SerializeField] public GameObject buttonPromptXB; //There might've been another way to do this instead of having a whole script for this one thing
    public bool areaPatched = false;
    private void Start()
    {
        buttonPromptXB.SetActive(false);
    }
}
