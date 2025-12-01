using UnityEngine;

public class EnemyBody : MonoBehaviour
{
    [SerializeField] private DamageSource damageSource;

    [SerializeField] private float health = 100;
    public float Health {  get { return health; } set {  health = value; } }

    [SerializeField] private float attackRange = 20f;
    public float AttackRange { get {  return attackRange; } set {  attackRange = value; } }

    public void TakeDamage(float damage)
    {
        
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        this.gameObject.SetActive(false);
    }

    private void Attack()
    {
        Ray r = new Ray(gameObject.transform.position, gameObject.transform.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, attackRange))
        {
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out DamageSource ds))
        {
            if(ds.DamageTarget == DamageSource.DamageType.Enemy)
            {
                TakeDamage(ds.DamageVal);
            }
            else if(ds.DamageTarget != DamageSource.DamageType.Enemy)
            {
                Debug.LogError("DamageTarget is set to the wrong value to damage this");
            }
        }
    }

    
    
}
