using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject healthPackPrefab;
    public GameObject bombPrefab;

    public float healthPackSpawnRate = 0f; // 
    public float bombSpawnRate = 0f; // 

    private float lastHealthPackSpawn = 0;
    private float lastBombSpawn = 0;

    void Update()
    {
        if (Time.time - lastHealthPackSpawn >= healthPackSpawnRate)
        {
            SpawnHealthPack();
            lastHealthPackSpawn = Time.time;
        }

        if (Time.time - lastBombSpawn >= bombSpawnRate)
        {
            SpawnBomb();
            lastBombSpawn = Time.time;
        }
    }

    void SpawnHealthPack()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Debug.Log("Spawn HealPack");
            Vector2 spawnPos = GetRandomSpawnPosition(player.transform.position);
            Instantiate(healthPackPrefab, spawnPos, Quaternion.identity);
        }
    }

    void SpawnBomb()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            Debug.Log("Spawn Bomb");
            Vector2 spawnPos = GetRandomSpawnPosition(player.transform.position);
            Instantiate(bombPrefab, spawnPos, Quaternion.identity);
        }
    }

    Vector2 GetRandomSpawnPosition(Vector2 playerPos)
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float distance = Random.Range(5f, 10f);
        return playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;
    }
}
