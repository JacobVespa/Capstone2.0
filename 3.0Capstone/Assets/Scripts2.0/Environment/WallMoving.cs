using Unity.VisualScripting;
using UnityEngine;

public class WallMoving : MonoBehaviour
{
    [SerializeField] public float wallMoveSpeed;
    [SerializeField] public float floorMoveSpeed;
    [SerializeField] private bool isLooping = true;
    
    private bool hasFinished = false;
    private Vector3 startPosition;

    //offset wall and floor position for despawning
    private float wallOffsetPos = -43.5f;
    private float floorOffsetPos = -47f;

    //random spawn rate
    private float spawnRate = 0.2f; //20%

    [SerializeField] private GameObject[] gems;

    private void Start()
    {
        startPosition = transform.position;
        SpawnGems();
    }

    void Update()
    {
        if (isLooping)
        {
            WallMovement();
        }
        else if (!hasFinished)
        {
            WallMovement();
            
            if(gameObject.CompareTag("Wall"))
            {
                HasFinishedCheck(wallOffsetPos);
            }
            if(gameObject.CompareTag("floor"))
            {
                HasFinishedCheck(floorOffsetPos);
            }
            
        }
    }

    private void WallMovement()
    {
        if(gameObject.CompareTag("Wall"))
        {
            transform.position -= new Vector3(0, wallMoveSpeed, 0) * Time.deltaTime;
            DistanceCheck(wallOffsetPos, 43.5f);
        }
        if(gameObject.CompareTag("Floor"))
        {
            transform.position -= new Vector3(0, floorMoveSpeed, 0) * Time.deltaTime;
            DistanceCheck(floorOffsetPos, 47f);
        }
    }

    private void DistanceCheck(float offsetPos, float yPos)
    {
        //check if destroy based on offset
        if (isLooping && transform.position.y <= offsetPos)
        {
            transform.position = new Vector3(0, yPos, 0);
            SpawnGems();
        }
    }

    private void HasFinishedCheck(float offsetPos)
    {
        // Calculate total distance traveled from start position
        float distanceTraveled = startPosition.y - transform.position.y;
        if (distanceTraveled >= Mathf.Abs(offsetPos))
        {
            hasFinished = true;
        }
    }

    private void SpawnGems()
    {
        foreach(GameObject gem in gems)
        {
            float randChance = Random.value; //generates value between 0.0 and 1.0
            if(randChance <= spawnRate && !gem.activeSelf)
            {
                gem.SetActive(true);
            }
        }
    }

    // Public method to toggle looping at runtime
    public void SetLooping(bool looping)
    {
        isLooping = looping;
        if(!looping)
        {
            hasFinished = false;
            startPosition = transform.position; // Reset start position when disabling looping
        }
    }
}