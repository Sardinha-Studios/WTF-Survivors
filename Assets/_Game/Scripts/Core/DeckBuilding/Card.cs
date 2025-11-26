using System.Collections.Generic;
using UnityEngine;

// ===== ENUMS =====
public enum CardType
{
    Weapon,
    Skill,
    Passive
}

public enum Rarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(fileName = "NewCard", menuName = "Deckbuilding/Card")]
public class Card : ScriptableObject
{
    public string cardName;
    [TextArea] public string description;
    public Sprite icon;
    public CardType type;
    public Rarity rarity;

    [Header("Stats")]
    public float damageBonus;
    public float attackSpeedBonus;
    public float healthBonus;
    public float moveSpeedBonus;
    public float critChanceBonus;

    [Header("Upgrade")]
    public int maxLevel = 5;
    public Card upgradeCard;
}
