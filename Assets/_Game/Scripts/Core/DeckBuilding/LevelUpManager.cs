using System;
using System.Collections.Generic;
using UnityEngine;
using SardinhaStudios;

public class LevelUpManager : Singleton<LevelUpManager>
{
    public PlayerDeck playerDeck;
    public GameObject levelUpPanel;
    public CardSelectionUI[] cardOptions; // Array de 3 botões de UI
    
    int playerLevel = 1;
    int currentXP = 0;
    int xpToNextLevel = 100;
    
    void Start()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
    }
    
    public void GainXP(int amount)
    {
        currentXP += amount;
        
        if (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }
    
    void LevelUp()
    {
        playerLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);
        
        Debug.Log($"Level Up! Nível {playerLevel}");
        ShowLevelUpOptions();
    }
    
    void ShowLevelUpOptions()
    {
        Time.timeScale = 0f; // Pausa o jogo
        
        List<Card> options = playerDeck.GetAvailableCardsForSelection(3);
        
        for (int i = 0; i < cardOptions.Length; i++)
        {
            if (i < options.Count)
            {
                cardOptions[i].SetCard(options[i], this);
                cardOptions[i].gameObject.SetActive(true);
            }
            else
            {
                cardOptions[i].gameObject.SetActive(false);
            }
        }
        
        if (levelUpPanel != null)
            levelUpPanel.SetActive(true);
    }
    
    public void SelectCard(Card card)
    {
        playerDeck.AddCard(card);
        
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);
        
        Time.timeScale = 1f; // Despausa o jogo
    }
}
