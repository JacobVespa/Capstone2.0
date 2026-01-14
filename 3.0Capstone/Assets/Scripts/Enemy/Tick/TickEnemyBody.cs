using UnityEngine;

public class TickEnemyBody : EnemyBody
{





    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack(GameObject target)
    {
        if (attackCooldown < attackRate) { return; }
        base.Attack(target);

        if(target.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr))
        {
            dr.Attacked(damageSource);
        }
        else { Debug.LogError("No damage receiver found on target"); }
    }

    public void DropOnRig(Vector2 targetPos)
    {

    }
}
