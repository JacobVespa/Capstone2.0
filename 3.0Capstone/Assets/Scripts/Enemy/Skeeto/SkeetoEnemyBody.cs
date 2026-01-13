using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class SkeetoEnemyBody : EnemyBody
{
    [SerializeField] private float projSpeed;

    [Header("Required Bullet Compoennts")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform objectPool;
    private List<Projectile> bullets = new List<Projectile>();
    [SerializeField] private Transform fireLocation;

    
    

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("yes");
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack(GameObject target)
    {
        if (attackCooldown < attackRate) { return; }
        attackCooldown = 0;

        Projectile bullet = GetProjectile();

        bullet.gameObject.transform.position = fireLocation.position;
        Vector2 projDir = (target.transform.position - fireLocation.position).normalized;
        projDir = SetBloom(projDir);

        bullet.Fire(projSpeed, projDir);
    }

    private Projectile GetProjectile()
    {
        //if(bullets.Count == 0) {  }
        
        foreach(Projectile b in bullets)
        {
            if (!b.active)
            {
                return b;
            }
        }
        return CreateProjectile();


        //return null;
    }

    
    
    private Projectile CreateProjectile()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, objectPool);

        Projectile bulletScript = bulletObj.GetComponent<Projectile>();
        bulletScript.DamageSource = damageSource;

        bullets.Add(bulletScript);

        return bulletScript;
    }

    private Vector2 SetBloom(Vector2 dir)
    {
        float xChange = Random.Range(0.9f, 1.1f); ;
        float yChange = Random.Range(0.9f, 1.1f); ;

        dir.x = dir.x * xChange;
        dir.y = dir.y * yChange;

        return dir;
    }

    
}
