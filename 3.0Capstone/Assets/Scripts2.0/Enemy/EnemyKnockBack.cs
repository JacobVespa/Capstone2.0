using UnityEngine;

public class EnemyKnockBack : MonoBehaviour
{
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = this.GetComponent<Rigidbody2D>();
    }

    public void KnockBack(Transform drillTransform, float knockbackForce)
    {
        Vector2 direction = (transform.position - drillTransform.position).normalized;
        rb.angularVelocity = knockbackForce * direction.x;
        Debug.Log("Knockback applied");
    }
}
