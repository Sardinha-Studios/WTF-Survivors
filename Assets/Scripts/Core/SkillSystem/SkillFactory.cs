using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Factory responsável por criar os comportamentos de habilidades
/// </summary>
public class SkillFactory
{
    private AttackManager attackManager;
    private Dictionary<SkillType, ISkillBehavior> behaviorCache = new Dictionary<SkillType, ISkillBehavior>();

    // Referências necessárias para criar os behaviors
    private ObjectPoolParticles fireBreathPool;
    private Transform effectsParent;
    private ParticleSystem arrowPrefab;
    private Transform enemiesParent;
    private Transform firePoint;
    private Transform fireDirection;
    private DamageTick auraPrefab;
    private DamageTick thunderPrefab;
    private DamageArea meteorPrefab;
    private BoxCollider2D meteorSpawnArea;

    public void Initialize(AttackManager manager, SkillFactoryConfig config)
    {
        attackManager = manager;
        
        fireBreathPool = config.fireBreathPool;
        effectsParent = config.effectsParent;
        arrowPrefab = config.arrowPrefab;
        enemiesParent = config.enemiesParent;
        firePoint = config.firePoint;
        fireDirection = config.fireDirection;
        auraPrefab = config.auraPrefab;
        thunderPrefab = config.thunderPrefab;
        meteorPrefab = config.meteorPrefab;
        meteorSpawnArea = config.meteorSpawnArea;
    }

    public ISkillBehavior CreateBehavior(SkillType skillType)
    {
        // Verifica se já existe uma instância no cache
        if (behaviorCache.ContainsKey(skillType))
            return behaviorCache[skillType];

        ISkillBehavior behavior = null;

        switch (skillType)
        {
            case SkillType.FireBreath:
                behavior = new FireBreathBehavior(fireBreathPool, effectsParent);
                break;

            case SkillType.Arrow:
                behavior = new ArrowBehavior(arrowPrefab, enemiesParent, firePoint, fireDirection);
                break;

            case SkillType.Aura:
                behavior = new AuraBehavior(auraPrefab);
                break;

            case SkillType.Thunder:
                behavior = new ThunderBehavior(thunderPrefab);
                break;

            case SkillType.Meteor:
                behavior = new MeteorBehavior(meteorPrefab, meteorSpawnArea);
                break;

            default:
                Debug.LogError($"Skill type {skillType} not implemented!");
                return null;
        }

        // Armazena no cache para reutilização
        behaviorCache[skillType] = behavior;
        return behavior;
    }

    public void ClearCache()
    {
        behaviorCache.Clear();
    }
}

/// <summary>
/// Configuração para o SkillFactory
/// </summary>
[System.Serializable]
public class SkillFactoryConfig
{
    [Header("Fire Breath")]
    public ObjectPoolParticles fireBreathPool;
    public Transform effectsParent;

    [Header("Arrow")]
    public ParticleSystem arrowPrefab;
    public Transform enemiesParent;
    public Transform firePoint;
    public Transform fireDirection;

    [Header("Aura")]
    public DamageTick auraPrefab;

    [Header("Thunder")]
    public DamageTick thunderPrefab;

    [Header("Meteor")]
    public DamageArea meteorPrefab;
    public BoxCollider2D meteorSpawnArea;
}