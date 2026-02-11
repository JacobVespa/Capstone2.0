using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class BigTickEnemtAI : TickEnemyAI
{
    

    public GameObject locationList;

    private List<Transform> moveLocations;

    private Transform pastDest;

    private List<Transform> inRadius;

    [SerializeField] private CircleCollider2D detection;



    protected override void Awake()
    {
        
        base.Awake();
        if(locationList != null)
        {
            moveLocations = new List<Transform>(locationList.GetComponentsInChildren<Transform>());

        }
        
        if(detection != null) { detection.enabled = false; }
    }


    protected override void AIFlowChart()
    {

        switch (behaviour)
        {
            case Behaviour.Spawning:
                if (!body.dropping) { StartCoroutine(body.DropOnRig(dropPos));}
                break;
            case Behaviour.Moving:
                ApproachTarget();
                if (targetLoc.transform == transform) { Debug.Log("there"); }
                break;
            case Behaviour.Ready:
                if(detection.enabled == false) { detection.enabled = true; }
                if (agent.CanDealDamage)
                {
                    behaviour = Behaviour.Attacking;
                }
                break;
            case Behaviour.Attacking:
                TryAttackTarget();
                break;
            case Behaviour.CoolDown:
                if (body.CheckCoolDown()) { MoveToBottomOfQueue(); }
                break;
            case Behaviour.Dead:
                if (CheckDeathPlayed()) { DestroyEnemy(); }
                break;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            
            //behaviour = Behaviour.Moving;
            //FindNewTarget();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {

            if(behaviour != Behaviour.Moving) {
                body.attackNotif.SetActive(false);
                behaviour = Behaviour.Moving;
                SelectNewTarget(detection.ClosestPoint(collision.transform.position));
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 0)
        {
            //targetLoc = null;
            behaviour = Behaviour.Ready;
        }
    }

    protected override void ApproachTarget()
    {
        base.ApproachTarget();

        if (Mathf.Abs(Vector2.Distance(transform.position, targetLoc.transform.position)) <= 0.5)
        {
            //targetLoc = null;
            behaviour = Behaviour.Ready;
        }
    }

    private void SelectNewTarget(Vector2 player)
    {
        pastDest = targetLoc.transform;
        List<Transform> dirOptions = GetTargetOptions(player);

        

        if (dirOptions.Count > 0)
        {
            targetLoc = dirOptions[Random.Range(0, dirOptions.Count)].gameObject;
        }
        else { targetLoc = moveLocations[Random.Range(0, moveLocations.Count)].gameObject; }
    }

    private List<Transform> GetTargetOptions(Vector2 player)
    {
        List<Transform> targetOptions = new List<Transform>();

        

        return targetOptions;
    }
}

/*
        /Debug.Log((Vector2)transform.position); 
        Debug.Log(moveDir);
        Debug.Log(transform.position.y + " " + moveDir.y);
        //Vector2 moveDir = new Vector2();
        moveDir = ((Vector2)transform.position - moveDir).normalized;
        //moveDir = moveDir.normalized;
        moveDir.x = Mathf.Round(moveDir.x);
        moveDir.y = Mathf.Round(moveDir.y);
        
        Debug.LogError(moveDir);

        foreach (Transform t in moveLocations)
        {
            
            if (t.gameObject == targetLoc ) { continue; }
            if(pastDest != null && t.gameObject == pastDest ) { continue; }

            Vector2 pointDir = new Vector2();
            pointDir = (transform.position - t.position).normalized;
            pointDir.x = Mathf.Round(pointDir.x);
            pointDir.y = Mathf.Round(pointDir.y);

            Debug.Log("-");
            Debug.Log(t.name);
            Debug.Log(pointDir);
            
            if(moveDir.x == pointDir.x) { continue;  }
            if (moveDir.y == pointDir.y) { continue; }
            //if (moveDir.x != 0 || moveDir.x == pointDir.x) { continue; }
            //if (moveDir.y == 0 || moveDir.y == pointDir.y) { continue; }

            Debug.Log("added");

            targetOptions.Add(t);
        }

        */
