using Unity.VisualScripting;
using UnityEngine;

public class TreadMoving : MonoBehaviour
{
    [SerializeField] private GameObject[] TreadSprites;
    [SerializeField] public float TreadMoveSpeed;
    [SerializeField] private bool isLooping = true;

    private bool hasFinished = false;
    private bool isPaused = false;
    private GameflowManager gameflowManager;
    private Level currentLevel;
    private Vector3 startPosition;

    //offset wall and floor position for despawning
    private float TreadOffsetPos = 7f;

    private void Start()
    {
        startPosition = transform.position;
        gameflowManager = FindFirstObjectByType<GameflowManager>();
        currentLevel = gameflowManager.CurrentLevel;
    }

    void Update()
    {
        if (isPaused) return;

        if (isLooping)
        {
            TreadMovement();
        }
        else if (!hasFinished)
        {
            TreadMovement();

            if (gameObject.CompareTag("Tread"))
            {
                HasFinishedCheck(TreadOffsetPos);
            }
        }
    }

    private void TreadMovement()
    {
        if (gameObject.CompareTag("Tread"))
        {
           if (currentLevel is GoldRushLevel goldRushLevel)
           {
                transform.position -= new Vector3(0, TreadMoveSpeed, 0) * Time.deltaTime;
                DistanceCheck(-9f, -15f);
           }
           else if (currentLevel is ScrollerLevel scrollerLevel)
           {
                transform.position -= new Vector3(0, TreadMoveSpeed, 0) * Time.deltaTime;
                DistanceCheck(TreadOffsetPos, -6f);
           }      
        }
    }

    private void DistanceCheck(float offsetPos, float yPos)
    {
        if (isLooping && transform.position.y >= offsetPos)
        {
            transform.position = new Vector3(0, yPos, 0);
        }
    }

    private void HasFinishedCheck(float offsetPos)
    {
        float distanceTraveled = startPosition.y - transform.position.y;
        if (distanceTraveled >= Mathf.Abs(offsetPos))
        {
            hasFinished = true;
        }
    }

    public void SetLooping(bool looping)
    {
        isLooping = looping;

        if (!looping)
        {
            hasFinished = false;
            startPosition = transform.position;
        }
    }

    public void Pause()
    {
        isPaused = true;
    }

    public void Play()
    {
        isPaused = false;
    }

    public float GetTreadSpeed()
    {
        if(isPaused)
        {
            return 0f;
        }
        else
        {
            return TreadMoveSpeed;
        }
    }

}