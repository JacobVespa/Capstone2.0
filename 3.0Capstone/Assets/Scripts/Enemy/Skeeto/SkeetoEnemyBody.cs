using UnityEngine;

public class SkeetoEnemyBody : EnemyBody
{

    [SerializeField] private GameObject objectPool;
    private GameObject[] bullets;
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

        Fire(target);

    }

    private void Fire(GameObject target)
    {
        Debug.Log("fired");
    }
}
