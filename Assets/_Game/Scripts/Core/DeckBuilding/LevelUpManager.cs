using System;
using System.Collections.Generic;
using UnityEngine;
using SardinhaStudios;
using UnityEngine.UI;

public class LevelUpManager : Singleton<LevelUpManager>
{
    [SerializeField] private PlayerDeck playerDeck;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private Slider levelProgressBar;
    [SerializeField] private CardSelectionUI[] cardOptions;

    private int playerLevel = 1;
    private int currentXP = 0;
    private int xpToNextLevel = 100;

    private Queue<int> pendingLevelUps = new Queue<int>();
    private bool isShowingLevelUp = false;

    void Start()
    {
        if (levelUpPanel != null)
            levelUpPanel.SetActive(false);

        UpdateLevelProgressBar();
    }

    private void UpdateLevelProgressBar()
    {
        if (levelProgressBar != null)
        {
            levelProgressBar.maxValue = xpToNextLevel;
            levelProgressBar.value = currentXP;
        }
    }

    public PlayerDeck GetPlayerDeck()
    {
        return playerDeck;
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        UpdateLevelProgressBar();

        // Check for multiple level ups
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }

        // If does not show UI and has pending level ups, shows the first one
        if (!isShowingLevelUp && pendingLevelUps.Count > 0)
        {
            ShowNextLevelUp();
        }
    }

    void LevelUp()
    {
        playerLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.2f);

        pendingLevelUps.Enqueue(playerLevel);
        Debug.Log($"Level Up! Level {playerLevel} - {pendingLevelUps.Count} pending level up(s)");
    }

    void ShowNextLevelUp()
    {
        if (pendingLevelUps.Count == 0)
            return;

        isShowingLevelUp = true;
        Time.timeScale = 0f; // Pausa game

        int level = pendingLevelUps.Dequeue();
        Debug.Log($"Showing level up options for level {level}");

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

        isShowingLevelUp = false;

        // If has pending level ups, show the next one
        if (pendingLevelUps.Count > 0)
        {
            ShowNextLevelUp();
        }
        else
        {
            // Unpause when there are no more pending level ups
            Time.timeScale = 1f;
        }

        UpdateLevelProgressBar();
    }
}
