using System.Collections;
using UnityEngine;

public class MeleeEnemyBody : EnemyBody
{

    [SerializeField] private CircleCollider2D attackRange;
    [SerializeField] private float attackRangeVal = 2;
    public float AttackRangeVal { get { return attackRangeVal; } set {  attackRangeVal = value; attackRange.radius = attackRangeVal; } }


    

    

    protected override void Awake()
    {
        base.Awake();
        attackRange.radius = attackRangeVal;
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    public override void Attack(GameObject target)
    {
        if (attackTimer < attackStartUp) { return; }


        base.Attack(target);
        StartCoroutine(AttackAnim(2.5f));

        if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver damageTarget))
        {
            damageTarget.Attacked(damageSource);
        }
        else{ Debug.LogError("no IDamage Receiver found on target");}

    }

    

    

    

    
    

}
