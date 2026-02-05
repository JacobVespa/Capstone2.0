using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BigTickEnemtAI : TickEnemyAI
{
    

    public GameObject locationList;

    private List<Transform> moveLocations;

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
                SelectNewTarget(collision.transform);
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

        Debug.Log(Mathf.Abs(Vector2.Distance(transform.position, targetLoc.transform.position)));

        if (Mathf.Abs(Vector2.Distance(transform.position, targetLoc.transform.position)) <= 0.5)
        {
            //targetLoc = null;
            behaviour = Behaviour.Ready;
        }
    }

    private void SelectNewTarget(Transform pos)
    {
        List<Transform> dirOptions = new List<Transform>();

        Vector2 moveDir = new Vector2();
        moveDir = (transform.position - pos.position).normalized;
        moveDir.x = Mathf.Round(moveDir.x);
        moveDir.y = Mathf.Round(moveDir.y);

        foreach (Transform t in moveLocations)
        {
            if(t.gameObject == targetLoc) { continue; }
            Vector2 pointDir = new Vector2();
            pointDir = (transform.position - t.position).normalized;
            pointDir.x = Mathf.Round(pointDir.x);
            pointDir.y = Mathf.Round(pointDir.y);

            if (moveDir.x != 0 && moveDir.x == pointDir.x && pointDir.x == 0) { continue; }
            if (moveDir.y != 0 && moveDir.y == pointDir.y) { continue; }

            dirOptions.Add(t);
            
        }


        if (dirOptions.Count > 0)
        {
            targetLoc = dirOptions[Random.Range(0, dirOptions.Count)].gameObject;
        }
        else { targetLoc = moveLocations[Random.Range(0, moveLocations.Count)].gameObject; }
        Debug.LogError(targetLoc.name);
    }
}
