using Unity.VisualScripting;
using UnityEngine;

public class WallMoving : MonoBehaviour
{
    [SerializeField] public float wallMoveSpeed = 3f;
    [SerializeField] private GameObject[] gems;

    //offset wall position for despawning
    private float offsetPos = -43.5f;

    void Update()
    {
        transform.position -= new Vector3(0, wallMoveSpeed, 0) * Time.deltaTime;

        //check if destroy based on offset
        if(transform.position.y <= offsetPos)
        {
            //Destroy(gameObject);
            transform.position = new Vector3(0, 43.5f, 0);
            foreach (GameObject gem in gems)
            {
                Resource resource = gem.GetComponent<Resource>();
                resource.Respawn();
            }
        }
    }

}
