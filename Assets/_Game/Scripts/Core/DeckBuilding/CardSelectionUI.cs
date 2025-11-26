using System.Collections.Generic;
using UnityEngine;

public class CardSelectionUI : MonoBehaviour
{
    public UnityEngine.UI.Image iconImage;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI descriptionText;
    public TMPro.TextMeshProUGUI levelText;
    public UnityEngine.UI.Button button;
    
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
            int currentLevel = manager.playerDeck.GetCardLevel(card);
            if (currentLevel > 0)
                levelText.text = $"Nível {currentLevel} → {currentLevel + 1}";
            else
                levelText.text = "NOVO";
        }
        
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => manager.SelectCard(card));
        }
    }
}