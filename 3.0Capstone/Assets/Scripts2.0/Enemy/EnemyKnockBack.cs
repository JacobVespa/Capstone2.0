using System.Collections;
using UnityEngine;
using YourNamespace;

public class EnemyKnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private MeleeEnemyAI meleeAI;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAI = GetComponent<MeleeEnemyAI>();
    }

    public void KnockBack(Transform drillTransform, float knockbackForce, float stunTime)
    {
        meleeAI.behaviour = EnemyAI.Behaviour.Knockback;
        StartCoroutine(StunTimer(stunTime));
        Vector2 direction = (transform.position - drillTransform.position).normalized;
        rb.linearVelocityX = knockbackForce * direction.x;
        Debug.Log("Knockback applied");
    }

    IEnumerator StunTimer(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        rb.linearVelocityX = 0;
        meleeAI.behaviour = EnemyAI.Behaviour.Moving;
    }
}
