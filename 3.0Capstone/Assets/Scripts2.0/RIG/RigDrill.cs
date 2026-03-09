using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RigDrill : MonoBehaviour
{
    public Transform drillPoint;
    public float knockbackForce = 5f;
    public float stunTime = 0.5f;
    public float drillRadius = 1f;
    public LayerMask enemyLayer;

    private DamageSource damageSource;

    private void Start()
    {
        damageSource = GetComponent<DamageSource>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        KnockBackEnemies();
    }

    public void KnockBackEnemies()
    {
        Debug.LogError("OUCH!");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(drillPoint.position, drillRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<EnemyKnockBack>().KnockBack(transform, knockbackForce, stunTime);
                enemies[i].GetComponent<EnemyBody>().Attacked(damageSource);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(drillPoint.position, drillRadius);
    }
}
