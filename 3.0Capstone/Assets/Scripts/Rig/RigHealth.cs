using UnityEngine;

public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private float health = 100;
    public float Health { get { return health; } set {  health = value; } }

    public void Attacked(DamageSource d)
    {
        if (d.DamageTarget == DamageSource.DamageType.Player)
        {
            TakeDamage(d.DamageVal);
        }
        
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        this.gameObject.SetActive(false);
    }
}
