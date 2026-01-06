using UnityEngine;

public class MovePlane : MonoBehaviour
{
    public float speed;
    public GameObject wall;

    // Update is called once per frame
    void Update()
    {
        float distance = Time.deltaTime * speed;
        wall.transform.position += Vector3.back * distance;
    }
}