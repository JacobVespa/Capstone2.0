using System.Collections;
using UnityEditor;
using UnityEngine;

/*  MeleeEnemyBody is mainly for letting melee enemies attack
 * 
 *  contains:
 *  - varibles for the attack range and its collider
 *  - a method to initiate an attack on a game object
 */

namespace YourNamespace // Add this if you're using namespaces elsewhere
{
public class MeleeEnemyBody : EnemyBody
{
    [SerializeField] private CircleCollider2D attackRange;
    [SerializeField] private float attackRangeVal = 2;
    
    public float AttackRangeVal { get { return attackRangeVal; } set { attackRangeVal = value; attackRange.radius = attackRangeVal; } }

    protected override void Awake()
    {
        base.Awake();
        if(attackRange != null)
            {
                attackRange.radius = attackRangeVal;
            }
           // set the range collider radius equal to what the attackRangeVal is
    }

    void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            this.Health = DifficultyManager.Instance.GrubHealth;
        }
        
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    //  tries to get a an IDamageReceiver from the gameobject argument and attacks it (no raycast or pysical interaction to attack)
    public override void Attack(GameObject target)
    {
        if (attackTimer < attackStartUp) { return; }


        base.Attack(target);
        StartCoroutine(GrubAttack());
        audioSource.clip = attackClip;
        audioSource.Play();
        if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver damageTarget))
        {
            damageTarget.Attacked(damageSource);
        }
        else { Debug.LogError("no IDamage Receiver found on target"); }

    }

    IEnumerator GrubAttack()
    {
        if (animator != null)
        {
            animator.SetBool("Attack", true);
            yield return new WaitForSeconds(0.25f); //can adjust the time on this
            animator.SetBool("Attack", false);
        }
    }

}
}