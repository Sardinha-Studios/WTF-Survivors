using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardSelectionUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI levelText;
    public Button button;

    private Card card;
    private LevelUpManager manager;

    public void SetCard(Card c, LevelUpManager m)
    {
        card = c;
        manager = m;

        if (iconImage != null && card.icon != null)
            iconImage.sprite = card.icon;

        if (nameText != null)
            nameText.text = card.cardName;

        if (descriptionText != null)
            descriptionText.text = card.description;

        if (levelText != null)
        {
            int currentLevel = manager.GetPlayerDeck().GetCardLevel(card);
            if (currentLevel > 0)
                levelText.text = $"Level {currentLevel} → {currentLevel + 1}";
            else
                levelText.text = "NEW";
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.SelectCard(card));
        }
    }
}