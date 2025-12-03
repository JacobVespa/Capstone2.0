using UnityEngine;



public class AttackTest : MonoBehaviour, IDamageReceiver
{
    public float hp = 10f;
    
    public void TakeDamage(float damage)
    {
        
        hp -= damage;
        if(hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        this.gameObject.SetActive(false);
    }

    public void Attacked(DamageSource d)
    {
        if (d.DamageTarget == DamageSource.DamageType.Player)
        {
            TakeDamage(d.DamageVal);
        }
    }

    
}
