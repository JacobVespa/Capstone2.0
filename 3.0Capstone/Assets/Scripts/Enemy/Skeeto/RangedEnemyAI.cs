using UnityEngine;
using UnityEngine.U2D;

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<RangedEnemyBody>(); }
        behaviour = Behaviour.Ready;
        
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
            case Behaviour.Ready:
                body.Attack(target);
                break;
        }
       
        
    }
}
