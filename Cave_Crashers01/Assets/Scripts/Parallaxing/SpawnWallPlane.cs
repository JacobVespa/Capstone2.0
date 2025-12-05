using UnityEngine;

public class SpawnWallPlane : MonoBehaviour
{
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private Transform spawnTransform;

    [SerializeField] private float spawnRate;
    [SerializeField] private int poolSize;

    private GameObject[] wallPool;
    private int nextPoolIndex = 0;
    private float nextFireTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializePool();
    }

    // Update is called once per frame
    void Update()
    {
        if (CanSpawn())
        {
            SpawnWall();
        }
    }

    private void InitializePool()
    {
        wallPool = new GameObject[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            GameObject wall = Instantiate(wallPrefab, spawnTransform);
            wall.SetActive(false);
            wallPool[i] = wall;
        }
    }

    private void SpawnWall()
    {
        GameObject wall = GetPooledObject();

        if (wall != null)
        {
            wall.transform.position = spawnTransform.position;
            wall.transform.rotation = spawnTransform.rotation;
            wall.SetActive(true);

            nextFireTime = Time.time + spawnRate;
        }
    }

    private bool CanSpawn()
    {
        return Time.time >= nextFireTime;
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < wallPool.Length; i++)
        {
            if (!wallPool[i].activeInHierarchy)
            {
                return wallPool[i];
            }
        }

        GameObject reusedProjectile = wallPool[nextPoolIndex];
        nextPoolIndex = (nextPoolIndex + 1) % poolSize;

        return reusedProjectile;
    }
}
