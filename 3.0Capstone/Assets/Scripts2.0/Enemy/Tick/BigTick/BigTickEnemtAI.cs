using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

public class BigTickEnemtAI : TickEnemyAI
{
    

    [SerializeField] private CircleCollider2D detection;

    public GameObject pointA;
    public GameObject pointB;

    protected override void Awake()
    {
        
        base.Awake();
       
        
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
                //ApproachTarget();
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

    protected override void ApproachTarget()
    {
        base.ApproachTarget();

        if (Mathf.Abs(Vector2.Distance(transform.position, targetLoc.transform.position)) <= 0.5)
        {
            //targetLoc = null;
            behaviour = Behaviour.Ready;
        }
    }

    private void FindEscapeDirection(Collider2D collision)
    {
        Vector2 pPos = Vector2.zero;
        
        if (collision.ClosestPoint(transform.position) != (Vector2)transform.position)
        {
            pPos = collision.ClosestPoint(transform.position);
        }
        else { pPos = collision.transform.position;}

        float escapeAngle = GetOppositeAngle(pPos);

        

        CheckDirection(escapeAngle);

        
    }

    private float GetOppositeAngle(Vector2 pPos)
    {
        Vector2 dirToPlay = (pPos - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(dirToPlay.y, dirToPlay.x);

        float opposite = (((angle * Mathf.Rad2Deg) + 180f) % 360f) * Mathf.Deg2Rad;
        
        return opposite;
    }

    private void CheckDirection(float angle)
    {
        float posAngle = (((angle * Mathf.Rad2Deg) + 30f) % 360) * Mathf.Deg2Rad;
        float negAngle = (((angle * Mathf.Rad2Deg) - 30f) % 360) * Mathf.Deg2Rad;
        int layer = LayerMask.GetMask("Default");
        Vector2 DV = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        Vector2 posDV = new Vector2(Mathf.Cos(posAngle), Mathf.Sin(posAngle));
        Vector2 negDV = new Vector2(Mathf.Cos(negAngle), Mathf.Sin(negAngle));

        RaycastHit2D posRay = Physics2D.Raycast(transform.position, posDV,layer);
        RaycastHit2D negRay = Physics2D.Raycast(transform.position, negDV,layer);

        Debug.Log(posRay.collider.name);
        Debug.Log(negRay.collider.name);

        Debug.DrawRay(transform.position, DV * 2, Color.black);
        Debug.DrawRay(transform.position, posDV * 2, Color.blue);
        Debug.DrawRay(transform.position, negDV * 2,  Color.magenta);
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
            FindEscapeDirection(collision);

            if (behaviour != Behaviour.Moving) {
                body.attackNotif.SetActive(false);
                behaviour = Behaviour.Moving;
                //FindEscapeDirection(detection.ClosestPoint(collision.transform.position));
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
