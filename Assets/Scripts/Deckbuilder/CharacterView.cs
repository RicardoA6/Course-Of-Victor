using UnityEngine;
using UnityEngine.UI;

public class CharacterView : MonoBehaviour
{
    public Image PortraitImage;
    public Text NameText;
    public Image HealthFillImage;
    public Text HealthText;
    public GameObject TurnHighlight;

    private static readonly Color AliveColor = new Color(0.45f, 0.45f, 0.45f);
    private static readonly Color DeadColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);

    public void Setup(Player player, bool isCurrentTurn)
    {
        int currentHealth = Mathf.Max(0, player.CurrentHealth);

        NameText.text = player.PlayerName;
        HealthText.text = $"{currentHealth}/{player.MaxHealth}";
        HealthFillImage.fillAmount = player.MaxHealth > 0 ? (float)currentHealth / player.MaxHealth : 0f;
        PortraitImage.color = player.IsAlive ? AliveColor : DeadColor;
        TurnHighlight.SetActive(isCurrentTurn);
    }
}
