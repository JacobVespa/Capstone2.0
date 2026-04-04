using System.Collections;
using UnityEngine;
using YourNamespace;

public class EnemyKnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    private MeleeEnemyAI meleeAI;
    private Collider2D[] colliders;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meleeAI = GetComponent<MeleeEnemyAI>();
        colliders = GetComponentsInChildren<Collider2D>(); 
    }

    public void KnockBack(Transform drillTransform, float knockbackForce, float stunTime)
    {
        meleeAI.behaviour = EnemyAI.Behaviour.Knockback;
        foreach (Collider2D c in colliders)
        {
            c.enabled = false;
        }
        StartCoroutine(StunTimer(stunTime));
        Vector2 direction = (transform.position - drillTransform.position).normalized;
        rb.linearVelocity = knockbackForce * direction;
    }

    IEnumerator StunTimer(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);
        if (!meleeAI.CheckInView(0.99f))
        {
            Debug.LogError("offscreen");
            Destroy(gameObject);
        }
        rb.linearVelocityX = 0;
        foreach (Collider2D c in colliders)
        { 
            c.enabled = true; 
        }
        meleeAI.behaviour = EnemyAI.Behaviour.Moving;
    }
}
