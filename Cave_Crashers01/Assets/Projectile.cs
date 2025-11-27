using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float lifetime = 5f;
    
    private float spawnTime;

    private void OnEnable()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - spawnTime >= lifetime)
        {
            DisableProjectile();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        DisableProjectile();
    }

    private void DisableProjectile()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        gameObject.SetActive(false);
    }
}