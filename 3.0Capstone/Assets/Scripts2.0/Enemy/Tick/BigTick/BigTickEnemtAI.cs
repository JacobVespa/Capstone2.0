using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BigTickEnemtAI : TickEnemyAI
{
    

    public GameObject locationList;

    private List<Transform> moveLocations;

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
            behaviour = Behaviour.Moving;
            FindNewTarget();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 0)
        {
            targetLoc = null;
            behaviour = Behaviour.Ready;
        }
    }

    private void FindNewTarget()
    {
        targetLoc = moveLocations[Random.Range(0, moveLocations.Count)].gameObject;
    }
}
