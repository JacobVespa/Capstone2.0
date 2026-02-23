using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RigDrill : MonoBehaviour
{
    public float knockbackForce = 50f;
    public float drillRadius = 50f;
    public LayerMask enemyLayer;

    public void KnockBackEnemies()
    {
        Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
        Collider2D[] enemies = Physics2D.OverlapCircleAll(this.transform.position, drillRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].GetComponent<EnemyKnockBack>().KnockBack(transform, knockbackForce);
            }
        }
    }
}
