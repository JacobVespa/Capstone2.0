using System.Collections;
using UnityEngine;

namespace YourNamespace // Add this if you're using namespaces elsewhere
{
    public class MeleeEnemyBody : EnemyBody
    {
        [SerializeField] private CircleCollider2D attackRange;
        [SerializeField] private float attackRangeVal = 2;
        
        public float AttackRangeVal 
        { 
            get { return attackRangeVal; } 
            set 
            {  
                attackRangeVal = value; 
                attackRange.radius = attackRangeVal; 
            } 
        }

        protected override void Awake()
        {
            base.Awake();
            if (attackRange != null)
            {
                attackRange.radius = attackRangeVal;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void Attack(GameObject target)
        {
            if (attackTimer < attackStartUp) { return; }

            base.Attack(target);
            StartCoroutine(GrubAttack());

            if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver damageTarget))
            {
                damageTarget.Attacked(damageSource);
            }
            else 
            { 
                Debug.LogError("no IDamageReceiver found on target");
            }
        }

        private IEnumerator GrubAttack()
        {
            if (animator != null)
            {
                animator.SetBool("Attack", true);
                yield return new WaitForSeconds(0.25f);
                animator.SetBool("Attack", false);
            }
        }
    }
}