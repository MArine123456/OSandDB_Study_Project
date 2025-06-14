using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public float spawnRate = 2f;
    public float spawnDistance = 15f;
    public int maxEnemies = 100;

    [Header("적 프리팹")]
    public GameObject[] enemyPrefabs;

    private float lastSpawnTime;
    private Camera mainCamera;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        //CreateEnemyPrefabs();
    }

    void Update()
    {
        if (Time.time - lastSpawnTime >= spawnRate && activeEnemies.Count < maxEnemies)
        {
            SpawnEnemy();
            lastSpawnTime = Time.time;
        }

        // 시간에 따른 스폰율 증가
        spawnRate = Mathf.Max(0.1f, 2f - (GameManager.Instance.gameTime / 60f) * 0.1f);

        CleanupDestroyedEnemies();
    }

    [System.Obsolete]
    void SpawnEnemy()
    {
        gameObject.tag = "Enemy";

        Vector2 spawnPosition = GetRandomSpawnPosition();

        // 적 타입 선택 (시간에 따라 강한 적 등장 확률 증가)
        EnemyType spawnType = SelectEnemyType();
        GameObject enemyPrefab = enemyPrefabs[(int)spawnType];

        if (spawnType == EnemyType.Hexagon)
        {
            // 육각형은 8마리씩 원형으로 스폰
            SpawnHexagonGroup(spawnPosition);
        }
        else
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }

    void SpawnHexagonGroup(Vector2 centerPosition)
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 2f;
            Vector2 spawnPos = centerPosition + offset;

            GameObject enemy = Instantiate(enemyPrefabs[3], spawnPos, Quaternion.identity);
            activeEnemies.Add(enemy);
        }
    }

    EnemyType SelectEnemyType()
    {
        float gameTime = GameManager.Instance.gameTime;

        if (gameTime < 30f) return EnemyType.Circle;
        if (gameTime < 60f) return Random.value < 0.7f ? EnemyType.Circle : EnemyType.Square;
        if (gameTime < 120f)
        {
            float rand = Random.value;
            if (rand < 0.4f) return EnemyType.Circle;
            if (rand < 0.7f) return EnemyType.Square;
            return EnemyType.Triangle;
        }

        // 2분 이후 모든 적 등장
        float rand2 = Random.value;
        if (rand2 < 0.3f) return EnemyType.Circle;
        if (rand2 < 0.5f) return EnemyType.Square;
        if (rand2 < 0.8f) return EnemyType.Triangle;
        return EnemyType.Hexagon;
    }

    [System.Obsolete]
    Vector2 GetRandomSpawnPosition()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player == null) return Vector2.zero;

        Vector2 playerPos = player.transform.position;

        // 플레이어 주변 원형으로 스폰
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        Vector2 spawnPos = playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnDistance;

        return spawnPos;
    }

    void CleanupDestroyedEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }
}
