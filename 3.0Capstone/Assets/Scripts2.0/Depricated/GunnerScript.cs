using UnityEngine;
using UnityEngine.InputSystem;

public class GunnerScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private GameObject projectilePrefab;
    
    [Header("Shooting Settings")]
    [SerializeField] private float projectileSpeed = 50f;
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private int poolSize = 20;
    
    private GameObject[] ammoPool;
    private int nextPoolIndex = 0;
    private float nextFireTime = 0f;
    private PlayerInput input;

    private void Start()
    {
        InitializePool();
    }

    private void Update()
    {
        if (input != null && CanShoot() && input.actions["Attack"].IsPressed())
        {
            Shoot();
        }
    }

    private void InitializePool()
    {
        ammoPool = new GameObject[poolSize];
        
        for (int i = 0; i < poolSize; i++)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform);
            projectile.SetActive(false);
            ammoPool[i] = projectile;
        }
    }

    private bool CanShoot()
    {
        return Time.time >= nextFireTime;
    }

    private void Shoot()
    {
        GameObject projectile = GetPooledProjectile();
        
        if (projectile != null)
        {
            projectile.transform.position = barrelTransform.position;
            projectile.transform.rotation = barrelTransform.rotation;
            projectile.SetActive(true);
            
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = barrelTransform.forward * projectileSpeed;
            }
            
            nextFireTime = Time.time + fireRate;
        }
    }

    private GameObject GetPooledProjectile()
    {
        // First try to find an inactive projectile
        for (int i = 0; i < ammoPool.Length; i++)
        {
            if (!ammoPool[i].activeInHierarchy)
            {
                return ammoPool[i];
            }
        }
        
        // If pool exhausted, reuse the oldest (cycle through array)
        GameObject reusedProjectile = ammoPool[nextPoolIndex];
        nextPoolIndex = (nextPoolIndex + 1) % poolSize;
        
        return reusedProjectile;
    }

    public void SetPlayerInput(PlayerInput playerInput)
    {
        input = playerInput;
    }

    public void ClearPlayerInput()
    {
        input = null;
    }
}