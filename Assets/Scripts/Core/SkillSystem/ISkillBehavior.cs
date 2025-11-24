using UnityEngine;

/// <summary>
/// Interface base para comportamentos de habilidades
/// </summary>
public interface ISkillBehavior
{
    void Initialize(SkillData data, AttackManager attackManager);
    void Activate(SkillLevelData levelData);
    void Deactivate();
    void UpdateLevel(SkillLevelData newLevelData);
}