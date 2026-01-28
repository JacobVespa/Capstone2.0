using UnityEngine;

public class RepairPatch : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Activate()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = true;
    }

    
}
