using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class RigDrill : MonoBehaviour
{
    public Transform drillPoint;
    public float knockbackForce = 100f;
    //public float stunTime = 0.5f;
    //public float drillRadius = 1f;
    //public LayerMask enemyLayer;

    private DamageSource damageSource;

    private void Start()
    {
        damageSource = GetComponent<DamageSource>();   
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.layer == 6)
        {
            
            //Debug.LogError("drill hit");
            HitEnemies(collision.gameObject);
        }
    }


    public void HitEnemies(GameObject enemy)
    {
        EnemyBody body = enemy.GetComponent<EnemyBody>();
        Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();

        Vector2 angle;

        if(enemy.transform.position.x >= transform.position.x)
        {
            angle = new Vector2(1, 1);
        }
        else { angle = new Vector2(-1, 1); }

            
        StartCoroutine(body.Knockback(angle,knockbackForce));

        body.Attacked(damageSource);
        
    }

    /*
    public void KnockBackEnemies()
    {
        
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
    */
}
