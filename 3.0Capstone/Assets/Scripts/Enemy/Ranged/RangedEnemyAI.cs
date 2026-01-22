using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    
    [SerializeField] private float AttackRange = 10;


    

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<RangedEnemyBody>(); }
        behaviour = Behaviour.Moving;
        
    }

    private void Start()
    {
        AddToAttackQueue();
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
                if (agent.CanDealDamage)
                {
                    behaviour = Behaviour.Attacking;
                }
                break;
            case Behaviour.Attacking:
                body.Attack(target);
                break;
            case Behaviour.CoolDown:
                MoveToBottomOfQueue();
                break;
        }
    }

    protected override void ApproachTarget()
    {
        base.ApproachTarget();


        body.InputDir = moveInput;

        if(moveInput == Vector2.zero) { return; }

        RaycastHit2D[] r = Physics2D.RaycastAll(transform.position, moveInput,100,1);

        foreach(RaycastHit2D h in r)
        {
            if (h.collider.gameObject == target)
            {
                if(h.distance <= AttackRange)
                {
                    behaviour = Behaviour.Ready;
                    moveInput = Vector2.zero;
                }
            }
        }
    }
}
