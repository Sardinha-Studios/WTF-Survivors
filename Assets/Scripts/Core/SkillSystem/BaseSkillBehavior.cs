using UnityEngine;
using System.Collections;

/// <summary>
/// Classe base abstrata para todos os comportamentos de habilidade
/// </summary>
public abstract class BaseSkillBehavior : ISkillBehavior
{
    protected SkillData skillData;
    protected AttackManager attackManager;
    protected SkillLevelData currentLevelData;
    protected Coroutine activeCoroutine;

    public virtual void Initialize(SkillData data, AttackManager manager)
    {
        skillData = data;
        attackManager = manager;
    }

    public virtual void Activate(SkillLevelData levelData)
    {
        currentLevelData = levelData;
    }

    public virtual void Deactivate()
    {
        if (activeCoroutine != null)
        {
            attackManager.StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }
    }

    public virtual void UpdateLevel(SkillLevelData newLevelData)
    {
        currentLevelData = newLevelData;
    }
}

// ============================================
// FIRE BREATH BEHAVIOR
// ============================================
public class FireBreathBehavior : BaseSkillBehavior
{
    private ObjectPoolParticles fireBreathPool;
    private Transform effectsParent;
    private Vector3 defaultScale;

    public FireBreathBehavior(ObjectPoolParticles pool, Transform parent)
    {
        fireBreathPool = pool;
        effectsParent = parent;
        if (pool.PoolList.Count > 0)
            defaultScale = pool.PoolList[0].transform.localScale;
    }

    public override void Activate(SkillLevelData levelData)
    {
        base.Activate(levelData);
        
        foreach (var item in fireBreathPool.PoolList)
        {
            var area = item.GetComponentInChildren<DamageArea>();
            if (area != null)
                area.damage = levelData.GetFinalDamage();
            item.transform.localScale = defaultScale * levelData.damageArea;
        }

        activeCoroutine = attackManager.StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(currentLevelData.cooldown);
        }
    }

    private void ExecuteAttack()
    {
        var fireBreath = fireBreathPool.GetInstance();
        var tempParent = fireBreath.transform.parent;

        fireBreath.transform.SetParent(effectsParent);
        fireBreath.transform.localPosition = Vector3.zero;
        fireBreath.transform.localEulerAngles = new Vector3(90, 0, 0);
        fireBreath.transform.SetParent(tempParent);
        
        fireBreath.gameObject.SetActive(true);
        fireBreath.Play();
    }
}

// ============================================
// ARROW BEHAVIOR
// ============================================
public class ArrowBehavior : BaseSkillBehavior
{
    private ParticleSystem arrowPrefab;
    private Transform enemiesParent;
    private Transform firePoint;
    private Transform fireDirection;
    private float shootDistance = 12f;

    public ArrowBehavior(ParticleSystem prefab, Transform enemies, Transform point, Transform direction)
    {
        arrowPrefab = prefab;
        enemiesParent = enemies;
        firePoint = point;
        fireDirection = direction;
    }

    public override void Activate(SkillLevelData levelData)
    {
        base.Activate(levelData);
        activeCoroutine = attackManager.StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(currentLevelData.cooldown);
        }
    }

    private void ExecuteAttack()
    {
        attackManager.StartCoroutine(ShootProjectiles());
    }

    private IEnumerator ShootProjectiles()
    {
        for (int i = 0; i < currentLevelData.projectiles; i++)
        {
            var closestEnemy = FindClosestEnemy();
            
            if (closestEnemy != null && Vector3.Distance(closestEnemy.position, attackManager.transform.position) < shootDistance)
                fireDirection.LookAt(closestEnemy);

            var arrowEffect = Object.Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
            // var projectileMover = arrowEffect.GetComponent<HS_ProjectileMover2D>();
            
            // if (projectileMover != null)
            //     projectileMover.damage = currentLevelData.GetFinalDamage();

            arrowEffect.gameObject.SetActive(true);
            arrowEffect.Play();

            if (i < currentLevelData.projectiles - 1)
                yield return new WaitForSeconds(0.1f);
        }
    }

    private Transform FindClosestEnemy()
    {
        if (enemiesParent == null || enemiesParent.childCount == 0)
            return null;

        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < enemiesParent.childCount; i++)
        {
            Transform target = enemiesParent.GetChild(i);
            float distance = Vector3.Distance(attackManager.transform.position, target.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = target;
            }
        }

        return closestTarget;
    }
}

// ============================================
// AURA BEHAVIOR
// ============================================
public class AuraBehavior : BaseSkillBehavior
{
    private DamageTick auraPrefab;
    private Vector3 defaultScale;

    public AuraBehavior(DamageTick prefab)
    {
        auraPrefab = prefab;
        defaultScale = prefab.transform.localScale;
    }

    public override void Activate(SkillLevelData levelData)
    {
        base.Activate(levelData);
        
        auraPrefab.damage = levelData.GetFinalDamage();
        auraPrefab.transform.localScale = defaultScale * levelData.damageArea;
        auraPrefab.gameObject.SetActive(true);
    }

    public override void Deactivate()
    {
        base.Deactivate();
        auraPrefab.gameObject.SetActive(false);
    }

    public override void UpdateLevel(SkillLevelData newLevelData)
    {
        base.UpdateLevel(newLevelData);
        auraPrefab.damage = newLevelData.GetFinalDamage();
        auraPrefab.transform.localScale = defaultScale * newLevelData.damageArea;
    }
}

// ============================================
// THUNDER BEHAVIOR
// ============================================
public class ThunderBehavior : BaseSkillBehavior
{
    private DamageTick thunderPrefab;

    public ThunderBehavior(DamageTick prefab)
    {
        thunderPrefab = prefab;
    }

    public override void Activate(SkillLevelData levelData)
    {
        base.Activate(levelData);
        activeCoroutine = attackManager.StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(currentLevelData.cooldown);
        }
    }

    private void ExecuteAttack()
    {
        var thunderEffect = Object.Instantiate(thunderPrefab, attackManager.transform.position, thunderPrefab.transform.rotation);
        thunderEffect.damage = currentLevelData.GetFinalDamage();
        thunderEffect.gameObject.SetActive(true);
        
        var ps = thunderEffect.GetComponent<ParticleSystem>();
        if (ps != null) ps.Play();
    }
}

// ============================================
// METEOR BEHAVIOR
// ============================================
public class MeteorBehavior : BaseSkillBehavior
{
    private DamageArea meteorPrefab;
    private BoxCollider2D spawnArea;
    private Vector3 defaultScale;

    public MeteorBehavior(DamageArea prefab, BoxCollider2D area)
    {
        meteorPrefab = prefab;
        spawnArea = area;
        defaultScale = prefab.transform.localScale;
    }

    public override void Activate(SkillLevelData levelData)
    {
        base.Activate(levelData);
        activeCoroutine = attackManager.StartCoroutine(AutoAttackRoutine());
    }

    private IEnumerator AutoAttackRoutine()
    {
        while (true)
        {
            ExecuteAttack();
            yield return new WaitForSeconds(currentLevelData.cooldown);
        }
    }

    private void ExecuteAttack()
    {
        attackManager.StartCoroutine(SpawnMeteors());
    }

    private IEnumerator SpawnMeteors()
    {
        for (int i = 0; i < currentLevelData.projectiles; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition();
            
            var meteorEffect = Object.Instantiate(meteorPrefab, spawnPosition, meteorPrefab.transform.rotation);
            meteorEffect.transform.localScale = defaultScale * currentLevelData.damageArea;
            meteorEffect.damage = currentLevelData.GetFinalDamage();
            meteorEffect.gameObject.SetActive(true);
            
            var ps = meteorEffect.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();

            if (i < currentLevelData.projectiles - 1)
                yield return new WaitForSeconds(0.1f);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector2 minBounds = spawnArea.bounds.min;
        Vector2 maxBounds = spawnArea.bounds.max;
        
        float randomX = Random.Range(minBounds.x, maxBounds.x);
        float randomY = Random.Range(minBounds.y, maxBounds.y);
        
        return new Vector3(randomX, randomY, 0f);
    }
}