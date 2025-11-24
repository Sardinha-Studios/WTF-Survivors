using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MoreMountains.TopDownEngine;

/// <summary>
/// Gerenciador central do sistema de habilidades
/// </summary>
public class SkillManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PauseGame pauseManager;
    [SerializeField] private AttackManager attackManager;
    
    [Header("Skill Database")]
    [SerializeField] private List<SkillData> skillDatabase;
    
    [Header("UI")]
    [SerializeField] private MainMenuUI menuManager;
    [SerializeField] private NewPowerUpgrade upgradeButtonPrefab;
    [SerializeField] private Transform upgradeButtonContainer;
    [SerializeField] private int maxUpgradeOptions = 3;

    [Header("Settings")]
    [SerializeField] private bool showDebugLogs = false;

    // Runtime data
    private Dictionary<string, SkillData> activeSkills = new Dictionary<string, SkillData>();
    private Dictionary<string, ISkillBehavior> skillBehaviors = new Dictionary<string, ISkillBehavior>();
    private SkillFactory skillFactory;
    private int playerLevel = 1;

    // Events
    public System.Action<SkillData> OnSkillUnlocked;
    public System.Action<SkillData> OnSkillUpgraded;
    public System.Action<List<SkillData>> OnUpgradeOptionsShown;

    private void Awake()
    {
        InitializeSkillFactory();
    }

    private IEnumerator Start()
    {
        yield return WaitForPlayerInitialization();
        InitializeSkillSystem();
    }

    private void InitializeSkillFactory()
    {
        skillFactory = new SkillFactory();
    }

    private IEnumerator WaitForPlayerInitialization()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => LevelManager.Instance != null && LevelManager.Instance.Players.Count > 0);

        var character = LevelManager.Instance.Players[0];
        attackManager = character.GetComponent<AttackManager>();
    }

    private void InitializeSkillSystem()
    {
        // Cria cópias runtime das skills
        foreach (var originalSkill in skillDatabase)
        {
            var runtimeCopy = originalSkill.CreateRuntimeCopy();
            activeSkills[runtimeCopy.skillId] = runtimeCopy;
        }

        // Ativa skills previamente desbloqueadas
        ActivateUnlockedSkills();

        Log("Skill system initialized");
    }

    private void ActivateUnlockedSkills()
    {
        foreach (var skill in activeSkills.Values)
        {
            if (skill.isUnlocked)
            {
                ActivateSkill(skill);
            }
        }
    }

    /// <summary>
    /// Mostra opções de upgrade para o jogador
    /// </summary>
    public void ShowUpgradeOptions()
    {
        List<SkillData> eligibleSkills = GetEligibleSkills();

        if (eligibleSkills.Count == 0)
        {
            Log("No eligible skills for upgrade");
            CloseUpgradeMenu();
            return;
        }

        DisplayUpgradeOptions(eligibleSkills);
        OnUpgradeOptionsShown?.Invoke(eligibleSkills);
    }

    private List<SkillData> GetEligibleSkills()
    {
        return activeSkills.Values
            .Where(skill => CanUpgradeSkill(skill))
            .ToList();
    }

    private bool CanUpgradeSkill(SkillData skill)
    {
        // Verifica se tem níveis disponíveis
        if (!skill.CanUpgrade())
            return false;

        // Verifica requisitos de nível do jogador
        if (playerLevel < skill.requiredPlayerLevel)
            return false;

        // Verifica requisitos de outras skills
        if (!CheckSkillRequirements(skill))
            return false;

        return true;
    }

    private bool CheckSkillRequirements(SkillData skill)
    {
        foreach (var requiredSkillId in skill.requiredSkillIds)
        {
            if (!activeSkills.ContainsKey(requiredSkillId) || !activeSkills[requiredSkillId].isUnlocked)
                return false;
        }
        return true;
    }

    private void DisplayUpgradeOptions(List<SkillData> eligibleSkills)
    {
        ClearUpgradeButtons();
        
        // Randomiza e limita as opções
        var selectedSkills = eligibleSkills
            .OrderBy(x => Random.value)
            .Take(maxUpgradeOptions)
            .ToList();

        // Cria os botões de upgrade
        foreach (var skill in selectedSkills)
        {
            CreateUpgradeButton(skill);
        }
    }

    private void CreateUpgradeButton(SkillData skill)
    {
        var upgradeButton = Instantiate(upgradeButtonPrefab, upgradeButtonContainer);
        
        upgradeButton.button.onClick.RemoveAllListeners();
        upgradeButton.button.onClick.AddListener(() => OnUpgradeSelected(skill));

        UpdateUpgradeButtonUI(upgradeButton, skill);
    }

    private void UpdateUpgradeButtonUI(NewPowerUpgrade upgradeButton, SkillData skill)
    {
        upgradeButton.nameText.text = $"{skill.skillName} - Level {skill.currentLevel + 1}";
        upgradeButton.descriptionText.text = GetSkillDescription(skill);
        upgradeButton.imageIcon.sprite = skill.iconSprite;
        upgradeButton.newSkill.SetActive(!skill.isUnlocked);
    }

    private string GetSkillDescription(SkillData skill)
    {
        var nextLevel = skill.GetNextLevelData();
        if (nextLevel == null)
            return skill.skillDescription;

        // Adiciona informações do próximo nível
        return $"{skill.skillDescription}\n\n" +
               $"Damage: {nextLevel.GetFinalDamage()}\n" +
               $"Cooldown: {nextLevel.cooldown}s";
    }

    private void OnUpgradeSelected(SkillData skill)
    {
        if (skill.isUnlocked)
        {
            UpgradeSkill(skill);
        }
        else
        {
            UnlockSkill(skill);
        }

        CloseUpgradeMenu();
    }

    private void UnlockSkill(SkillData skill)
    {
        skill.isUnlocked = true;
        ActivateSkill(skill);
        OnSkillUnlocked?.Invoke(skill);
        
        Log($"Skill unlocked: {skill.skillName}");
    }

    private void UpgradeSkill(SkillData skill)
    {
        var behavior = skillBehaviors[skill.skillId];
        var nextLevelData = skill.GetNextLevelData();
        
        behavior.UpdateLevel(nextLevelData);
        
        skill.currentLevel++;
        skill.levels.RemoveAt(0);
        
        OnSkillUpgraded?.Invoke(skill);
        
        Log($"Skill upgraded: {skill.skillName} to level {skill.currentLevel}");
    }

    private void ActivateSkill(SkillData skill)
    {
        // Cria o comportamento se não existir
        if (!skillBehaviors.ContainsKey(skill.skillId))
        {
            var behavior = skillFactory.CreateBehavior(skill.skillType);
            if (behavior == null)
            {
                Debug.LogError($"Failed to create behavior for skill: {skill.skillName}");
                return;
            }

            behavior.Initialize(skill, attackManager);
            skillBehaviors[skill.skillId] = behavior;
        }

        // Ativa com os dados do primeiro nível
        var levelData = skill.GetNextLevelData();
        if (levelData != null)
        {
            skillBehaviors[skill.skillId].Activate(levelData);
            skill.currentLevel++;
            skill.levels.RemoveAt(0);
        }
    }

    private void CloseUpgradeMenu()
    {
        pauseManager?.TogglePause();
        menuManager?.ChangeWindow("HUD");
    }

    private void ClearUpgradeButtons()
    {
        foreach (Transform child in upgradeButtonContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Desativa uma skill específica
    /// </summary>
    public void DeactivateSkill(string skillId)
    {
        if (skillBehaviors.ContainsKey(skillId))
        {
            skillBehaviors[skillId].Deactivate();
            Log($"Skill deactivated: {skillId}");
        }
    }

    /// <summary>
    /// Desativa todas as skills ativas
    /// </summary>
    public void DeactivateAllSkills()
    {
        foreach (var behavior in skillBehaviors.Values)
        {
            behavior.Deactivate();
        }
        Log("All skills deactivated");
    }

    /// <summary>
    /// Obtém informações de uma skill específica
    /// </summary>
    public SkillData GetSkillData(string skillId)
    {
        return activeSkills.ContainsKey(skillId) ? activeSkills[skillId] : null;
    }

    /// <summary>
    /// Atualiza o nível do jogador (afeta requisitos de skills)
    /// </summary>
    public void SetPlayerLevel(int level)
    {
        playerLevel = level;
        Log($"Player level set to: {level}");
    }

    /// <summary>
    /// Reseta todas as skills para o estado inicial
    /// </summary>
    public void ResetAllSkills()
    {
        DeactivateAllSkills();
        
        foreach (var skill in activeSkills.Values)
        {
            skill.ResetRuntimeData();
        }
        
        skillBehaviors.Clear();
        Log("All skills reset");
    }

    private void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log($"[SkillManager] {message}");
    }

    private void OnDestroy()
    {
        DeactivateAllSkills();
        
        // Limpa as cópias runtime
        foreach (var skill in activeSkills.Values)
        {
            if (skill != null)
                Destroy(skill);
        }
        
        activeSkills.Clear();
        skillBehaviors.Clear();
    }

#if UNITY_EDITOR
    [ContextMenu("Show Upgrade Options (Test)")]
    private void TestShowUpgradeOptions()
    {
        if (Application.isPlaying)
            ShowUpgradeOptions();
    }

    [ContextMenu("Reset All Skills (Test)")]
    private void TestResetAllSkills()
    {
        if (Application.isPlaying)
            ResetAllSkills();
    }
#endif
}