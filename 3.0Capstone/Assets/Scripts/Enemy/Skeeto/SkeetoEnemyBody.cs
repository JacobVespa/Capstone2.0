using UnityEngine;

public class SkeetoEnemyBody : EnemyBody
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform objectPool;
    private GameObject[] bullets;
    [SerializeField] private Transform fireLocation;

    [SerializeField] private float projSpeed;
    

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

        GameObject bulletObj = GetProjectile();
        Projectile bulletScript = bulletObj.GetComponent<Projectile>();

        bulletObj.transform.position = fireLocation.position;

        Vector2 projDir = (target.transform.position - fireLocation.position).normalized;



        bulletScript.Fire(projSpeed, projDir);
    }

    private GameObject GetProjectile()
    {
        if(objectPool.childCount == 0) { return CreateProjectile(); }

        return null;
    }
    
    private GameObject CreateProjectile()
    {
        GameObject bullet = Instantiate(bulletPrefab, objectPool);

        bullet.GetComponent<Projectile>().DamageSource = damageSource;

        return bullet;
    }
}
