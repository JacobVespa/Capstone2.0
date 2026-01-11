using UnityEngine;

public class WallMoving : MonoBehaviour
{
    [SerializeField] public float wallMoveSpeed = 3;


    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, wallMoveSpeed, 0) * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Destroy"))
        {
            Destroy(gameObject);   
        }
    }

}
