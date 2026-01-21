using UnityEngine;
using UnityEngine.U2D;

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    
    [SerializeField] float AttackRange = 10;


    

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

        Debug.Log(moveInput);

        body.InputDir = moveInput;

        if(moveInput == Vector2.zero) { return; }
        totalDist = targetDist.magnitude;

        if (totalDist <= AttackRange) 
        {
            Debug.Log("lmao");
            behaviour = Behaviour.Ready; 
            moveInput = Vector2.zero;

        }
    }
}
