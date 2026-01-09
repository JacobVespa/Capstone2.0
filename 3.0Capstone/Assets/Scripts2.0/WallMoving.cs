using UnityEngine;

public class WallMoving : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, 3, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);   
        }
    }

}
