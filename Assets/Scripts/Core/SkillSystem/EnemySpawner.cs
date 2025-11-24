using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Spaw Setup")]
    [SerializeField] private List<EnemyControllerBase> enemyPrefabs;
    [SerializeField] private EnemyControllerBase bossPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int maxEnemies = 16;
    [SerializeField] private int maxEnemiesLimit = 30;
    [SerializeField] private float spawnInterval = 1.4f;
    [SerializeField] private float difficultyIncreaseInterval = 30f;
    [SerializeField] private float decreaseSpawInterval = 0.2f;
    [SerializeField] private int enemiesToAddPerIncrease = 2;
    [SerializeField] private int enemiesToActivateAura = 5;

    [Header("Events")]
    [Space] public UnityEvent OnBossDefeat;

    private Dictionary<int, int> sameTypeCounter;
    private List<BoxCollider2D> spawnAreas = new List<BoxCollider2D>();
    private int enableEnemys = 0;

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => LevelManager.Instance.Players.Count > 0);

        sameTypeCounter = new Dictionary<int, int>();
        for (int i = 0; i < enemyPrefabs.Count; i++)
        {
            sameTypeCounter.Add(i, 0);
        }

        var character = LevelManager.Instance.Players[0];
        spawnAreas = character.GetComponentInChildren<EnemySpawnAreas>().SpawnAreas;
        player = character.transform;

        InvokeRepeating(nameof(SpawnEnemy), 0f, spawnInterval);
        InvokeRepeating(nameof(IncreaseDifficulty), difficultyIncreaseInterval, difficultyIncreaseInterval);
    }

    private void SpawnEnemy()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < maxEnemies)
        {
            Vector3 spawnPoint = GetRandomSpawnPoint();
            var randIndex = GetRandomEnemyIndex();
            var randEnemy = enemyPrefabs[randIndex];

            var enemy = Instantiate(randEnemy, spawnPoint, randEnemy.transform.rotation, transform);
            enemy.SetTarget(player);

            // Aura counter to enemies of same type
            sameTypeCounter[randIndex] += 1;

            if (sameTypeCounter[randIndex] >= enemiesToActivateAura)
            {
                // Create active aura functon;
                // enemy.ActivateAura();
                sameTypeCounter[randIndex] = 0; // Reset aura counter
            }
        }
    }

    private void IncreaseDifficulty()
    {
        maxEnemies = Mathf.Clamp(maxEnemies + enemiesToAddPerIncrease, 0, maxEnemiesLimit);
        spawnInterval = Mathf.Clamp(spawnInterval - decreaseSpawInterval, 0.01f, 10);
        enableEnemys = Mathf.Clamp(enableEnemys + 1, 0, enemyPrefabs.Count);
    }

    public void SpawnBoss()
    {
        CancelInvoke(nameof(SpawnEnemy));
        CancelInvoke(nameof(IncreaseDifficulty));
        KillAllEnemies();

        Vector3 spawnPoint = GetRandomSpawnPoint();
        var boss = Instantiate(bossPrefab, spawnPoint, bossPrefab.transform.rotation, transform);
        AudioManager.Instance.Play("BossMusic", true);
        boss.SetTarget(player);

        boss.GetComponent<Health>().OnDeath += () => OnBossDefeat.Invoke();
    }

    private void KillAllEnemies()
    {
        var enemys = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemys.Length; i++)
        {
            var enemyHealth = enemys[i].GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.Kill();
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        var area = spawnAreas[Random.Range(0, spawnAreas.Count)];

        float minX = area.bounds.min.x;
        float maxX = area.bounds.max.x;
        float minY = area.bounds.min.y;
        float maxY = area.bounds.max.y;

        float randomX = Random.Range(minX, maxX);
        float randomY = Random.Range(minY, maxY);

        return new Vector3(randomX, randomY);
    }

    private int GetRandomEnemyIndex()
    {
        return Random.Range(0, enableEnemys);
    }
}
