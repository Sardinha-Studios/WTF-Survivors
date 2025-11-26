using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<Card> availableCards = new List<Card>(); // Todas as cartas do jogo
    public List<Card> currentDeck = new List<Card>(); // Cartas que o player possui
    private Dictionary<Card, int> cardLevels = new Dictionary<Card, int>();
    
    [Header("Player Stats")]
    public float damage = 10f;
    public float attackSpeed = 1f;
    public float health = 100f;
    public float moveSpeed = 5f;
    public float critChance = 5f;
    
    void Start()
    {
        UpdatePlayerStats();
    }
    
    public void AddCard(Card card)
    {
        if (!currentDeck.Contains(card))
        {
            currentDeck.Add(card);
            cardLevels[card] = 1;
            Debug.Log($"Nova carta adicionada: {card.cardName}");
        }
        else
        {
            LevelUpCard(card);
        }
        
        UpdatePlayerStats();
    }
    
    void LevelUpCard(Card card)
    {
        if (cardLevels.ContainsKey(card))
        {
            cardLevels[card]++;
            Debug.Log($"{card.cardName} agora está no nível {cardLevels[card]}");
            
            // Se atingir max level e tiver upgrade disponível
            if (cardLevels[card] >= card.maxLevel && card.upgradeCard != null)
            {
                currentDeck.Remove(card);
                cardLevels.Remove(card);
                AddCard(card.upgradeCard);
                Debug.Log($"{card.cardName} evoluiu para {card.upgradeCard.cardName}!");
            }
        }
    }
    
    public int GetCardLevel(Card card)
    {
        return cardLevels.ContainsKey(card) ? cardLevels[card] : 0;
    }
    
    void UpdatePlayerStats()
    {
        // Reset stats base
        damage = 10f;
        attackSpeed = 1f;
        health = 100f;
        moveSpeed = 5f;
        critChance = 5f;
        
        // Aplica bônus de todas as cartas
        foreach (var card in currentDeck)
        {
            int level = GetCardLevel(card);
            damage += card.damageBonus * level;
            attackSpeed += card.attackSpeedBonus * level;
            health += card.healthBonus * level;
            moveSpeed += card.moveSpeedBonus * level;
            critChance += card.critChanceBonus * level;
        }
        
        Debug.Log($"Stats atualizados - Dano: {damage}, HP: {health}, Velocidade: {moveSpeed}");
    }
    
    public List<Card> GetAvailableCardsForSelection(int count = 3)
    {
        List<Card> options = new List<Card>();
        List<Card> pool = new List<Card>();
        
        // Cria pool de cartas disponíveis
        foreach (var card in availableCards)
        {
            // Se já possui a carta, verifica se pode fazer upgrade
            if (currentDeck.Contains(card))
            {
                int currentLevel = GetCardLevel(card);
                if (currentLevel < card.maxLevel)
                {
                    // Adiciona múltiplas vezes baseado na raridade (comum aparece mais)
                    int weight = GetRarityWeight(card.rarity);
                    for (int i = 0; i < weight; i++)
                        pool.Add(card);
                }
            }
            else
            {
                // Carta nova - adiciona baseado na raridade
                int weight = GetRarityWeight(card.rarity);
                for (int i = 0; i < weight; i++)
                    pool.Add(card);
            }
        }
        
        // Seleciona cartas aleatórias do pool
        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            Card selected = pool[randomIndex];
            
            if (!options.Contains(selected))
            {
                options.Add(selected);
            }
            
            // Remove todas as instâncias desta carta do pool
            pool.RemoveAll(c => c == selected);
        }
        
        return options;
    }
    
    int GetRarityWeight(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => 10,
            Rarity.Rare => 5,
            Rarity.Epic => 2,
            Rarity.Legendary => 1,
            _ => 1
        };
    }
}
