using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RangedEnemyBody : EnemyBody
{
    [SerializeField] private float projSpeed;

    [Header("Required Bullet Compoennts")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform objectPool;
    private List<Projectile> bullets = new List<Projectile>();
    [SerializeField] private Transform fireLocation;

    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5;
    private Vector3 motion = Vector2.zero;
    private Vector2 inputDir = Vector2.zero;
    public Vector2 InputDir { get { return inputDir; } set { inputDir = value; } }

    private Animator skeetoAnims;
    

    protected override void Awake()
    {
        base.Awake();
        
    }


    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateMovemnet();
    }

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

    IEnumerator SkeetoAttack()
    {
        if(skeetoAnims != null) {
            skeetoAnims.SetBool("Attack", true);
            yield return new WaitForSeconds(0.25f); //can adjust the time on this
            skeetoAnims.SetBool("Attack", false);
        }
        
        
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

    /*
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
    */

    private Vector2 SetBloom(Vector2 dir)
    {
        float xChange = Random.Range(0.9f, 1.1f); ;
        float yChange = Random.Range(0.9f, 1.1f); ;

        dir.x = dir.x * xChange;
        dir.y = dir.y * yChange;

        return dir;
    }

    

    private void UpdateMovemnet()
    {
        HandleMovement();
        transform.position = (transform.position + (motion * Time.fixedDeltaTime));
    }

    private void HandleMovement()
    {
        if (inputDir == Vector2.zero) { motion = Vector2.zero; return; }

        motion = transform.TransformDirection(inputDir) * moveSpeed;
        inputDir = Vector2.zero;

    }

}
