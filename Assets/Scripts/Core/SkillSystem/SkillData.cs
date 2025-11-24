using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillData", menuName = "RogueLike/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Identification")]
    public string skillId;
    public SkillType skillType;
    
    [Header("UI")]
    public Sprite iconSprite;
    public string skillName = "Skill Name";
    [TextArea(3, 6)]
    public string skillDescription = "Skill Description";
    
    [Header("Progression")]
    public List<SkillLevelData> levels = new List<SkillLevelData>();
    public int maxLevel => levels.Count;
    
    [Header("Requirements")]
    public List<string> requiredSkillIds = new List<string>();
    public int requiredPlayerLevel = 1;
    
    [Header("Runtime Data - Don't Edit")]
    public bool isUnlocked = false;
    public int currentLevel = 0;
    
    public bool CanUpgrade() => currentLevel < maxLevel;
    public bool HasNextLevel() => currentLevel < levels.Count;
    public SkillLevelData GetCurrentLevelData() => currentLevel > 0 ? levels[currentLevel - 1] : null;
    public SkillLevelData GetNextLevelData() => HasNextLevel() ? levels[currentLevel] : null;
    
    public void ResetRuntimeData()
    {
        isUnlocked = false;
        currentLevel = 0;
    }
    
    public SkillData CreateRuntimeCopy()
    {
        var copy = Instantiate(this);
        copy.ResetRuntimeData();
        return copy;
    }
}

[System.Serializable]
public class SkillLevelData
{
    [Header("Basic Stats")]
    [Min(0)] public int baseDamage = 1;
    [Min(0)] public float damageMultiplier = 1f;
    
    [Header("Area & Range")]
    [Min(0)] public float damageArea = 1f;
    [Min(0)] public float attackRange = 10f;
    
    [Header("Timing")]
    [Min(0.1f)] public float cooldown = 1f;
    [Min(0)] public float duration = 0f;
    
    [Header("Projectiles")]
    public int projectiles = 1;
    public float projectileSpeed = 10f;
    
    [Header("Special Effects")]
    public List<StatusEffect> statusEffects = new List<StatusEffect>();
    
    public int GetFinalDamage() => Mathf.RoundToInt(baseDamage * damageMultiplier);
}

[System.Serializable]
public class StatusEffect
{
    public StatusEffectType type;
    public float value;
    public float duration;
}

public enum SkillType
{
    FireBreath,
    Arrow,
    Aura,
    Thunder,
    Meteor,
    Custom
}

public enum StatusEffectType
{
    Slow,
    Burn,
    Poison,
    Freeze,
    Stun
}