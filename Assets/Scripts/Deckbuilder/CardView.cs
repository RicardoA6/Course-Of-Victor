using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    public Image ArtworkImage;
    public Text NameText;
    public Text CostText;
    public Button CardButton;

    public void Setup(CardData card, bool interactable)
    {
        NameText.text = card.CardName;
        CostText.text = card.EnergyCost.ToString();
        ArtworkImage.color = GetPlaceholderColor(card);
        CardButton.interactable = interactable;
    }

    private Color GetPlaceholderColor(CardData card)
    {
        if (card.Effects.Any(e => e is DamageEffect))
        {
            return new Color(0.55f, 0.16f, 0.16f);
        }

        if (card.Effects.Any(e => e is HealEffect))
        {
            return new Color(0.18f, 0.45f, 0.2f);
        }

        if (card.Effects.Any(e => e is DrainEnergyEffect))
        {
            return new Color(0.4f, 0.2f, 0.5f);
        }

        if (card.Effects.Any(e => e is ApplyStatusEffectCard))
        {
            return new Color(0.2f, 0.35f, 0.55f);
        }

        return new Color(0.3f, 0.3f, 0.3f);
    }
}
