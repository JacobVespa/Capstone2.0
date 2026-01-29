using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

/*  RangedEnemyAI detemines the actions RangedEnemyBody take depending on what state they are in and when they transition states
 * 
 *  contains:
 *  - varibles for EnemyBody and attack range
 *  - AIFlowChart to tell the body what action to take
 *  - a method to check the distance between the enemy and the target with a raycast
 */

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    [SerializeField] private float AttackRange = 10;

    int layer_mask;

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<RangedEnemyBody>(); }
        behaviour = Behaviour.Moving;
        layer_mask = LayerMask.GetMask("RIG");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        switch (behaviour)
        {
            case Behaviour.Moving:
                ApproachTarget();

                break;
            case Behaviour.Ready:
                if (agent.CanDealDamage) { behaviour = Behaviour.Attacking; }
                break;
            case Behaviour.Attacking:
                TryAttackTarget();
                break;
            case Behaviour.CoolDown:
                if (body.CheckCoolDown()) { MoveToBottomOfQueue(); }
                break;
            case Behaviour.Dead:
                if (CheckDeathPlayed() && !body.CheckProjectiles()) { DestroyEnemy(); }
                break;
        }
    }

    protected override void ApproachTarget()
    {
        base.ApproachTarget();


        body.InputDir = moveInput;

        if(moveInput == Vector2.zero) { return; }

        CheckTargetDist();
    }

    private void TryAttackTarget()
    {
        if (!hasTarget) { return; }
        if (body.attackNotif.activeSelf == false) { body.attackNotif.SetActive(true); }
        body.Attack(targetLoc);
    }

    // shoots out a raycast on the default layers and checks if anyhit by them are the target 
    // than checks if its with in range based on where it hit
    // if it is in range added to the attack queue and behaviour set to ready
    private void CheckTargetDist()
    {

        
        RaycastHit2D[] r = Physics2D.RaycastAll(transform.position, attackTarget.transform.position - transform.position , 100, layer_mask);

        
        foreach (RaycastHit2D h in r)
        {
            if (h.collider.gameObject == attackTarget)
            {
                
                if (h.distance <= AttackRange)
                {
                    behaviour = Behaviour.Ready;
                    moveInput = Vector2.zero;
                    AddToAttackQueue();
                }
            }
        }
    }
}
