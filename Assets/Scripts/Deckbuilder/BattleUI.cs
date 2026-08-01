using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private const int MAX_LOG_LINES = 8;

    private Font uiFont;
    private Text statusText;
    private Text rosterText;
    private Text logText;
    private Transform handContainer;
    private Transform targetContainer;
    private GameObject targetPanel;
    private GameObject matchEndPanel;
    private Text matchEndText;

    private readonly List<string> logLines = new List<string>();
    private CardData pendingCard;

    private void Awake()
    {
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        uiFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        BuildUI();

        gameManager.OnRoundStarted += HandleRoundStarted;
        gameManager.OnTurnStarted += HandleTurnStarted;
        gameManager.OnStateChanged += HandleStateChanged;
        gameManager.OnMatchEnded += HandleMatchEnded;
    }

    private void OnDestroy()
    {
        if (gameManager == null)
        {
            return;
        }

        gameManager.OnRoundStarted -= HandleRoundStarted;
        gameManager.OnTurnStarted -= HandleTurnStarted;
        gameManager.OnStateChanged -= HandleStateChanged;
        gameManager.OnMatchEnded -= HandleMatchEnded;
    }

    // ---- Event handlers ----

    private void HandleRoundStarted(int roundNumber)
    {
        AppendLog($"--- Round {roundNumber} ---");
    }

    private void HandleTurnStarted(Player player)
    {
        pendingCard = null;
        targetPanel.SetActive(false);
        AppendLog($"{player.PlayerName}'s turn");
        RefreshAll();
    }

    private void HandleStateChanged()
    {
        RefreshAll();
    }

    private void HandleMatchEnded(Team winner)
    {
        handContainer.gameObject.SetActive(false);
        targetPanel.SetActive(false);

        string message = winner != null ? $"{winner.TeamName} wins the match!" : "The match ended in a draw.";
        matchEndText.text = message;
        matchEndPanel.SetActive(true);
        AppendLog(message);
    }

    // ---- Refreshing ----

    private void RefreshAll()
    {
        RefreshStatus();
        RefreshRoster();
        RefreshHand();
    }

    private void RefreshStatus()
    {
        Player current = gameManager.CurrentPlayer;
        statusText.text = current != null
            ? $"{current.PlayerName}'s turn — Energy {current.CurrentEnergy}/{current.MaxEnergy}"
            : string.Empty;
    }

    private void RefreshRoster()
    {
        StringBuilder builder = new StringBuilder();

        foreach (Team team in gameManager.Teams)
        {
            builder.AppendLine(team.TeamName + (team.IsDefeated ? " (defeated)" : string.Empty));

            foreach (Player player in team.Players)
            {
                string marker = player == gameManager.CurrentPlayer ? "> " : "   ";
                string status = player.IsAlive ? $"HP {player.CurrentHealth}/{player.MaxHealth}" : "DEFEATED";
                builder.AppendLine($"{marker}{player.PlayerName}: {status}");
            }
        }

        rosterText.text = builder.ToString();
    }

    private void RefreshHand()
    {
        ClearChildren(handContainer);

        Player current = gameManager.CurrentPlayer;
        if (current == null)
        {
            return;
        }

        foreach (CardData card in current.Deck.Hand)
        {
            CardData capturedCard = card;
            Button button = CreateButton(handContainer, $"{card.CardName}\n({card.EnergyCost})");
            button.interactable = current.CanPlay(card);
            button.onClick.AddListener(() => OnCardClicked(capturedCard));
        }

        CreateButton(handContainer, "End Turn").onClick.AddListener(gameManager.EndCurrentTurn);
    }

    // ---- Card / target selection ----

    private void OnCardClicked(CardData card)
    {
        if (gameManager.RequiresManualTarget(card))
        {
            pendingCard = card;
            ShowTargetSelection(card);
            return;
        }

        Player actor = gameManager.CurrentPlayer;
        if (gameManager.SubmitCardPlay(card, null))
        {
            AppendLog($"{(actor != null ? actor.PlayerName : "?")} used {card.CardName}");
        }
    }

    private void ShowTargetSelection(CardData card)
    {
        ClearChildren(targetContainer);

        foreach (Player candidate in gameManager.GetCandidateTargets(card, gameManager.CurrentPlayer))
        {
            Player capturedTarget = candidate;
            Button button = CreateButton(targetContainer, candidate.PlayerName);
            button.onClick.AddListener(() => OnTargetClicked(capturedTarget));
        }

        CreateButton(targetContainer, "Cancel").onClick.AddListener(CancelTargetSelection);

        targetPanel.SetActive(true);
    }

    private void OnTargetClicked(Player target)
    {
        Player actor = gameManager.CurrentPlayer;
        CardData card = pendingCard;

        if (gameManager.SubmitCardPlay(card, target))
        {
            AppendLog($"{(actor != null ? actor.PlayerName : "?")} used {card.CardName} on {target.PlayerName}");
        }

        pendingCard = null;
        targetPanel.SetActive(false);
    }

    private void CancelTargetSelection()
    {
        pendingCard = null;
        targetPanel.SetActive(false);
    }

    private void AppendLog(string line)
    {
        logLines.Add(line);
        while (logLines.Count > MAX_LOG_LINES)
        {
            logLines.RemoveAt(0);
        }

        logText.text = string.Join("\n", logLines);
    }

    private void ClearChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    // ---- UI construction ----

    private void BuildUI()
    {
        GameObject canvasGO = new GameObject("BattleCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280f, 720f);

        if (FindFirstObjectByType<EventSystem>() == null)
        {
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        RectTransform canvasRect = canvasGO.GetComponent<RectTransform>();

        statusText = CreateText(canvasRect, TextAnchor.MiddleCenter, 22,
            new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0.5f, 1f),
            new Vector2(0f, -10f), new Vector2(0f, 40f));

        rosterText = CreateText(canvasRect, TextAnchor.UpperLeft, 16,
            new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
            new Vector2(20f, -70f), new Vector2(300f, 280f));

        logText = CreateText(canvasRect, TextAnchor.UpperLeft, 14,
            new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
            new Vector2(-20f, -70f), new Vector2(300f, 280f));

        handContainer = CreateLayoutPanel(canvasRect, "HandPanel",
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 60f), new Vector2(0f, 100f)).transform;

        targetPanel = CreateLayoutPanel(canvasRect, "TargetPanel",
            new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 170f), new Vector2(0f, 60f));
        targetContainer = targetPanel.transform;
        targetPanel.SetActive(false);

        matchEndPanel = new GameObject("MatchEndPanel", typeof(RectTransform), typeof(Image));
        RectTransform matchEndRect = matchEndPanel.GetComponent<RectTransform>();
        matchEndRect.SetParent(canvasRect, false);
        matchEndRect.anchorMin = Vector2.zero;
        matchEndRect.anchorMax = Vector2.one;
        matchEndRect.offsetMin = Vector2.zero;
        matchEndRect.offsetMax = Vector2.zero;
        matchEndPanel.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
        matchEndText = CreateText(matchEndRect, TextAnchor.MiddleCenter, 36,
            Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
        matchEndPanel.SetActive(false);
    }

    private Text CreateText(RectTransform parent, TextAnchor textAlignment, int fontSize,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject go = new GameObject("Text", typeof(RectTransform), typeof(Text));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        Text text = go.GetComponent<Text>();
        text.font = uiFont;
        text.fontSize = fontSize;
        text.alignment = textAlignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        return text;
    }

    private GameObject CreateLayoutPanel(RectTransform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(HorizontalLayoutGroup));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = sizeDelta;

        HorizontalLayoutGroup layout = go.GetComponent<HorizontalLayoutGroup>();
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.spacing = 10f;
        layout.childAlignment = TextAnchor.MiddleCenter;

        return go;
    }

    private Button CreateButton(Transform parent, string label)
    {
        GameObject go = new GameObject("Button", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.SetParent(parent, false);
        rect.sizeDelta = new Vector2(140f, 50f);

        LayoutElement layoutElement = go.GetComponent<LayoutElement>();
        layoutElement.preferredWidth = 140f;
        layoutElement.preferredHeight = 50f;

        go.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        GameObject textGO = new GameObject("Text", typeof(RectTransform), typeof(Text));
        RectTransform textRect = textGO.GetComponent<RectTransform>();
        textRect.SetParent(rect, false);
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textGO.GetComponent<Text>();
        text.text = label;
        text.font = uiFont;
        text.fontSize = 14;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return go.GetComponent<Button>();
    }
}
