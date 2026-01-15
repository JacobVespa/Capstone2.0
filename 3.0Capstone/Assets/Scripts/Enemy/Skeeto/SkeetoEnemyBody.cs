using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SkeetoEnemyBody : EnemyBody
{
    [SerializeField] private float projSpeed;

    [Header("Required Bullet Compoennts")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform objectPool;
    private List<Projectile> bullets = new List<Projectile>();
    [SerializeField] private Transform fireLocation;

    private Animator skeetoAnims;
    

    protected override void Awake()
    {
        base.Awake();
        Debug.Log("yes");
    }

    private void Start()
    {
        skeetoAnims = GetComponentInChildren<Animator>();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack(GameObject target)
    {
        if (attackCooldown < attackRate) { return; }

        AttackQueueAgent agent = GetComponent<AttackQueueAgent>();
        if (agent != null && !agent.CanDealDamage) { return; }

        base.Attack(target);
        StartCoroutine(SkeetoAttack());

        Projectile bullet = GetProjectile();

        bullet.gameObject.transform.position = fireLocation.position;
        Vector2 projDir = (target.transform.position - fireLocation.position).normalized;
        projDir = SetBloom(projDir);

        bullet.Fire(projSpeed, projDir);
    }

    IEnumerator SkeetoAttack()
    {
        skeetoAnims.SetBool("Attack", true);
        yield return new WaitForSeconds(0.25f); //can adjust the time on this
        skeetoAnims.SetBool("Attack", false);
    }

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

    
    private Projectile CreateProjectile()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, objectPool);

        Projectile bulletScript = bulletObj.GetComponent<Projectile>();
        bulletScript.DamageSource = damageSource;

        bullets.Add(bulletScript);

        return bulletScript;
    }

    protected override IEnumerator DestroyObject()
    {
        while (true)
        {
            int inactiveProj = 0;
            foreach(Projectile b in bullets)
            {
                if (!b.active)
                {
                    inactiveProj++;
                }
            }
            if(inactiveProj == bullets.Count)
            {
                break;
            }

            

            
            yield return null;
        }

        
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
