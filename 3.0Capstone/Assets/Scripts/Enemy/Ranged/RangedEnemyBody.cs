using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/*  RangedEnemyBody is mainly for allowing ranged enemies to shoot projectiles
 * 
 *  contains
 *  - varibles needed instantiating and handling projectiles
 *  - methods for attacking, spawning projectiles, handling projectile object pool
 */

public class RangedEnemyBody : EnemyBody
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
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // shoots out a bullet at the target
    public override void Attack(GameObject target)
    {
        if (attackTimer < attackStartUp) { return; }
        
        base.Attack(target);
        
        StartCoroutine(SkeetoAttack());

        Projectile bullet = GetProjectile();

        bullet.gameObject.transform.position = fireLocation.position;
        Vector2 projDir = (target.transform.position - fireLocation.position).normalized;
        projDir = SetBloom(projDir);

        bullet.Fire(projSpeed, projDir);
    }

    private IEnumerator SkeetoAttack()
    {
        if (animator != null) 
        {
            animator.SetBool("Attack", true);
            yield return new WaitForSeconds(0.25f);
            animator.SetBool("Attack", false);
        }
    }

    // return an inactive bulelt from the object pool
    // if there are no bullets or inactive bulelts int he object pool
    // than create a new one and return that 
    private Projectile GetProjectile()
    {
        foreach(Projectile b in bullets)
        {
            if (!b.active)
            {
                return b;
            }
        }
        return CreateProjectile();
    }

    // creates a bulelt prefab in the object pool 
    private Projectile CreateProjectile()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, objectPool);

        Projectile bulletScript = bulletObj.GetComponent<Projectile>();
        bulletScript.DamageSource = damageSource;

        bullets.Add(bulletScript);

        return bulletScript;
    }

    // sets bloom on projectiel so they don't always travel in the exact same direction
    private Vector2 SetBloom(Vector2 dir)
    {
        float xChange = Random.Range(0.9f, 1.1f);
        float yChange = Random.Range(0.9f, 1.1f);

        dir.x = dir.x * xChange;
        dir.y = dir.y * yChange;

        return dir;
    }
}