using UnityEngine;
using UnityEngine.U2D;

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    private Vector2 targetDist = Vector3.zero;
    private float totalDist = 0;
    [SerializeField] float AttackRange = 10;


    private bool inRange = false;

    private Vector2 moveInput;

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

    private void ApproachTarget()
    {
        if (!hasTarget) { moveInput = Vector2.zero; return; }

        targetDist = target.transform.position - transform.position;

        totalDist = targetDist.magnitude;

        

        moveInput = new Vector2(targetDist.x, targetDist.y);
        moveInput = moveInput.normalized;

        body.InputDir = moveInput;

        if (totalDist <= AttackRange) 
        { 
            behaviour = Behaviour.Ready; 
            moveInput = Vector2.zero;

        }
    }
}
