using System;
using System.Collections;
using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// Gerenciador de ataques refatorado para trabalhar com o novo sistema de skills
/// </summary>
public class AttackManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private SkillFactoryConfig factoryConfig;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;
    [SerializeField] private float arrowShootDistance = 12f;

    private SkillFactory skillFactory;
    private Health health;
    private Transform closestEnemy;

    public SkillFactoryConfig FactoryConfig => factoryConfig;

    private void Awake()
    {
        InitializeComponents();
        InitializeFactory();
    }

    private void InitializeComponents()
    {
        health = GetComponent<Health>();
        
        if (health != null)
            health.OnDeath += HandleDeath;

        // Busca o transform dos inimigos automaticamente
        var enemySpawner = FindObjectOfType<EnemySpawner>();
        if (enemySpawner != null)
            factoryConfig.enemiesParent = enemySpawner.transform;
    }

    private void InitializeFactory()
    {
        skillFactory = new SkillFactory();
        skillFactory.Initialize(this, factoryConfig);
    }

    private void HandleDeath()
    {
        StopAllAttacks();
    }

    /// <summary>
    /// Para todos os ataques ativos
    /// </summary>
    public void StopAllAttacks()
    {
        CancelInvoke();
        StopAllCoroutines();
    }

    /// <summary>
    /// Obtém a factory de skills
    /// </summary>
    public SkillFactory GetSkillFactory()
    {
        return skillFactory;
    }

    /// <summary>
    /// Busca o inimigo mais próximo
    /// </summary>
    public Transform FindClosestEnemy()
    {
        if (factoryConfig.enemiesParent == null || factoryConfig.enemiesParent.childCount == 0)
            return null;

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < factoryConfig.enemiesParent.childCount; i++)
        {
            Transform target = factoryConfig.enemiesParent.GetChild(i);
            
            // Verifica se o inimigo está ativo
            if (!target.gameObject.activeInHierarchy)
                continue;

            float distance = Vector3.Distance(transform.position, target.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        closestEnemy = closestTarget;
        return closestTarget;
    }

    /// <summary>
    /// Obtém todos os inimigos ativos
    /// </summary>
    public Transform[] GetActiveEnemies()
    {
        if (factoryConfig.enemiesParent == null)
            return new Transform[0];

        var enemies = new System.Collections.Generic.List<Transform>();
        
        for (int i = 0; i < factoryConfig.enemiesParent.childCount; i++)
        {
            Transform enemy = factoryConfig.enemiesParent.GetChild(i);
            if (enemy.gameObject.activeInHierarchy)
                enemies.Add(enemy);
        }

        return enemies.ToArray();
    }

    /// <summary>
    /// Busca inimigos dentro de um raio específico
    /// </summary>
    public Transform[] GetEnemiesInRange(float range)
    {
        var allEnemies = GetActiveEnemies();
        var enemiesInRange = new System.Collections.Generic.List<Transform>();

        foreach (var enemy in allEnemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.position);
            if (distance <= range)
                enemiesInRange.Add(enemy);
        }

        return enemiesInRange.ToArray();
    }

    private void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebugGizmos)
            return;

        // Desenha o raio de alcance dos ataques
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, arrowShootDistance);

        // Desenha linha para o inimigo mais próximo
        if (closestEnemy != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, closestEnemy.position);
        }

        // Desenha a área de spawn de meteoros
        if (factoryConfig.meteorSpawnArea != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(
                factoryConfig.meteorSpawnArea.bounds.center, 
                factoryConfig.meteorSpawnArea.bounds.size
            );
        }
    }

    private void OnValidate()
    {
        // Validação no editor
        if (factoryConfig.fireBreathPool == null)
            Debug.LogWarning("Fire Breath Pool is not assigned!");
        
        if (factoryConfig.arrowPrefab == null)
            Debug.LogWarning("Arrow Prefab is not assigned!");
    }
#endif
}
