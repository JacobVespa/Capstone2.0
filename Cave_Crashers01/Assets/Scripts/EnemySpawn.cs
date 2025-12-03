using UnityEditor;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private GameObject target;
    [SerializeField] private float spawnInterval = 7.5f;
    [SerializeField] private float spawnTime;

    private void Start()
    {
        spawnTime = spawnInterval;
    }

    private void Update()
    {
        UpdateSpawnTime(Time.deltaTime);
        SpawnEnemy();
    }

    private void SpawnEnemy()
    {
        if (spawnTime < spawnInterval) { return; }
        
        GameObject spawnEnemy = Instantiate(enemyPrefab, transform.position, transform.rotation);
        if(target.activeSelf == true)
        {
            spawnEnemy.GetComponent<EnemyAI>().Target = target;
        }

        spawnTime = 0;


    }

    private void UpdateSpawnTime(float time)
    {
        spawnTime += time;
    }
}
