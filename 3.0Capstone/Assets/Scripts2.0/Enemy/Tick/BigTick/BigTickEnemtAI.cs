using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class BigTickEnemtAI : TickEnemyAI
{
    Vector2 escapeDir = Vector2.zero;

    [SerializeField] private CircleCollider2D detection;

    protected override void Awake()
    {
        
        base.Awake();
       
        
        if(detection != null) { detection.enabled = false; }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if(escapeDir != Vector2.zero)
        {
            
            body.InputDir = escapeDir;
        }
    }

    protected override void AIFlowChart()
    {

        switch (behaviour)
        {
            case Behaviour.Spawning:
                Debug.Log(body.dropping);
                if (!body.dropping) { StartCoroutine(body.DropOnRig(dropPos));Debug.Log("start"); }
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

        Vector2 dirToPlay = (pPos - (Vector2)transform.position).normalized;

        float angle = Mathf.Atan2(dirToPlay.y, dirToPlay.x);

        float escapeAngle = GetOppositeAngle(angle);

        (float angleA, float angleB) = CheckDirection(escapeAngle);
        
        if(angleA < angleB)
        {
            escapeAngle = Random.Range(angleB, escapeAngle+angleA);
        }
        else  escapeAngle = Random.Range(angleA, angleB);

        escapeDir = GetDirFromAngle(escapeAngle);
        //Debug.DrawRay(transform.position, GetDirFromAngle(escapeAngle));

    }

    private float GetOppositeAngle(float angle)
    {

        float opposite = (((angle * Mathf.Rad2Deg) + 180f) % 360f) * Mathf.Deg2Rad;
        
        return opposite;
    }

    private (float angleA, float angleB) GetOtherAngles(float angle, float offput)
    {
        float posAngle = (((angle * Mathf.Rad2Deg) + offput) % 360) * Mathf.Deg2Rad;
        float negAngle = (((angle * Mathf.Rad2Deg) - offput) % 360) * Mathf.Deg2Rad;

        return (posAngle, negAngle);
    }

    private Vector2 GetDirFromAngle(float angle)
    {
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private (float angleA, float angleB) CheckDirection(float angle)
    {

        (float angleA, float angleB) = GetOtherAngles(angle, 30);

        int layer = LayerMask.GetMask("Default");
        Vector2 DV = GetDirFromAngle(angle);
        Vector2 posDV = GetDirFromAngle(angleA);
        Vector2 negDV = GetDirFromAngle(angleB);

        RaycastHit2D posRay = Physics2D.Raycast(transform.position, posDV,Mathf.Infinity,layer);
        RaycastHit2D negRay = Physics2D.Raycast(transform.position, negDV,Mathf.Infinity,layer);


        if(posRay.distance <= 2 && negRay.distance <= 2)
        {
            //Debug.Log("flip All");
            (angleA, angleB) = GetOtherAngles(GetOppositeAngle(angle), 30);
        }
        else if (posRay.distance <= 2)
        {
            //Debug.Log("turn neg");
            (angleA, angleB) = GetOtherAngles(angleB, 30);
        }
        else if(negRay.distance <= 2)
        {
            //Debug.Log("turn pos");
            (angleA, angleB) = GetOtherAngles(angleA, 30);
        }

        //Debug.DrawRay(transform.position, DV * 2, Color.black);
        //Debug.DrawRay(transform.position, posDV * 2, Color.blue);
        //Debug.DrawRay(transform.position, negDV * 2, Color.magenta);

        return (angleA, angleB);


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
        if (collision.gameObject.layer == 8 && collision.TryGetComponent<PlayerInteract>(out PlayerInteract p))
        {

            //Debug.LogError("check");

            //FindEscapeDirection(collision);
            if (behaviour != Behaviour.Moving) {
                body.attackNotif.SetActive(false);
                behaviour = Behaviour.Moving;

                FindEscapeDirection(collision);
            }
            
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer == 0)
        {
            //targetLoc = null;
            behaviour = Behaviour.Ready;
            escapeDir = Vector2.zero;
        }
    }

}

