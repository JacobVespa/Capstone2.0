using UnityEngine;

public class SkeetoEnemyBody : EnemyBody
{


    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform fireLocation;

    public void Attack(GameObject target)
    {
        if(attackCooldown < attackRate) { return; }

        Debug.Log("attack");
        Fire(target);

        attackCooldown = 0;
    }

    private void Fire(GameObject target)
    {
        GameObject bullet = Instantiate(projectile, fireLocation.position,Quaternion.identity);
        bullet.SetActive(true);
    }
}
