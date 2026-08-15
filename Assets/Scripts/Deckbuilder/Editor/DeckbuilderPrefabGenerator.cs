using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class DeckbuilderPrefabGenerator
{
    private const string PREFAB_FOLDER = "Assets/Resources/Deckbuilder";

    [MenuItem("Deckbuilder/Generate Card And Character Prefabs")]
    public static void GeneratePrefabs()
    {
        EnsureFolder();
        GenerateCardPrefab();
        GenerateCharacterPrefab();
        AssetDatabase.SaveAssets();
        Debug.Log($"Deckbuilder prefabs generated at {PREFAB_FOLDER}. Open them from the Project window to restyle.");
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        if (!AssetDatabase.IsValidFolder(PREFAB_FOLDER))
        {
            AssetDatabase.CreateFolder("Assets/Resources", "Deckbuilder");
        }
    }

    private static Font UiFont => Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    private static void GenerateCardPrefab()
    {
        GameObject root = new GameObject("CardView", typeof(RectTransform), typeof(Image), typeof(Button));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(120f, 170f);
        root.GetComponent<Image>().color = new Color(0.12f, 0.12f, 0.12f, 1f);

        GameObject art = CreateChild(root.transform, "ArtPanel", typeof(Image));
        SetRect(art, new Vector2(0.05f, 0.35f), new Vector2(0.95f, 0.95f), Vector2.zero, Vector2.zero);
        Image artImage = art.GetComponent<Image>();
        artImage.color = new Color(0.4f, 0.4f, 0.4f);

        GameObject nameGO = CreateChild(root.transform, "NameText", typeof(Text));
        SetRect(nameGO, new Vector2(0.05f, 0.12f), new Vector2(0.95f, 0.34f), Vector2.zero, Vector2.zero);
        Text nameText = nameGO.GetComponent<Text>();
        nameText.font = UiFont;
        nameText.fontSize = 13;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.white;
        nameText.horizontalOverflow = HorizontalWrapMode.Wrap;

        GameObject costBadge = CreateChild(root.transform, "CostBadge", typeof(Image));
        RectTransform costRect = costBadge.GetComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0f, 1f);
        costRect.anchorMax = new Vector2(0f, 1f);
        costRect.pivot = new Vector2(0f, 1f);
        costRect.anchoredPosition = new Vector2(4f, -4f);
        costRect.sizeDelta = new Vector2(26f, 26f);
        costBadge.GetComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.9f);

        GameObject costTextGO = CreateChild(costBadge.transform, "CostText", typeof(Text));
        SetRect(costTextGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text costText = costTextGO.GetComponent<Text>();
        costText.font = UiFont;
        costText.fontSize = 14;
        costText.alignment = TextAnchor.MiddleCenter;
        costText.color = Color.white;

        CardView cardView = root.AddComponent<CardView>();
        cardView.ArtworkImage = artImage;
        cardView.NameText = nameText;
        cardView.CostText = costText;
        cardView.CardButton = root.GetComponent<Button>();

        PrefabUtility.SaveAsPrefabAsset(root, $"{PREFAB_FOLDER}/CardView.prefab");
        Object.DestroyImmediate(root);
    }

    private static void GenerateCharacterPrefab()
    {
        GameObject root = new GameObject("CharacterView", typeof(RectTransform), typeof(Image));
        root.GetComponent<RectTransform>().sizeDelta = new Vector2(160f, 90f);
        root.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.85f);

        GameObject highlight = CreateChild(root.transform, "TurnHighlight", typeof(Image));
        SetRect(highlight, Vector2.zero, Vector2.one, new Vector2(-3f, -3f), new Vector2(3f, 3f));
        highlight.transform.SetAsFirstSibling();
        highlight.GetComponent<Image>().color = new Color(1f, 0.85f, 0.2f, 0.9f);
        highlight.SetActive(false);

        GameObject portrait = CreateChild(root.transform, "Portrait", typeof(Image));
        RectTransform portraitRect = portrait.GetComponent<RectTransform>();
        portraitRect.anchorMin = new Vector2(0f, 0f);
        portraitRect.anchorMax = new Vector2(0.35f, 1f);
        portraitRect.offsetMin = new Vector2(4f, 4f);
        portraitRect.offsetMax = new Vector2(-2f, -4f);
        Image portraitImage = portrait.GetComponent<Image>();
        portraitImage.color = new Color(0.45f, 0.45f, 0.45f);

        GameObject nameGO = CreateChild(root.transform, "NameText", typeof(Text));
        RectTransform nameRect = nameGO.GetComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0.37f, 0.55f);
        nameRect.anchorMax = new Vector2(1f, 1f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = new Vector2(-4f, -2f);
        Text nameText = nameGO.GetComponent<Text>();
        nameText.font = UiFont;
        nameText.fontSize = 12;
        nameText.alignment = TextAnchor.UpperLeft;
        nameText.color = Color.white;
        nameText.horizontalOverflow = HorizontalWrapMode.Wrap;

        GameObject healthBackground = CreateChild(root.transform, "HealthBarBackground", typeof(Image));
        RectTransform healthBgRect = healthBackground.GetComponent<RectTransform>();
        healthBgRect.anchorMin = new Vector2(0.37f, 0.28f);
        healthBgRect.anchorMax = new Vector2(1f, 0.5f);
        healthBgRect.offsetMin = Vector2.zero;
        healthBgRect.offsetMax = new Vector2(-4f, 0f);
        healthBackground.GetComponent<Image>().color = new Color(0.25f, 0.05f, 0.05f);

        GameObject healthFillGO = CreateChild(healthBackground.transform, "HealthBarFill", typeof(Image));
        SetRect(healthFillGO, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image healthFillImage = healthFillGO.GetComponent<Image>();
        healthFillImage.color = new Color(0.15f, 0.6f, 0.2f);
        healthFillImage.type = Image.Type.Filled;
        healthFillImage.fillMethod = Image.FillMethod.Horizontal;
        healthFillImage.fillOrigin = (int)Image.OriginHorizontal.Left;
        healthFillImage.fillAmount = 1f;

        GameObject healthTextGO = CreateChild(root.transform, "HealthText", typeof(Text));
        RectTransform healthTextRect = healthTextGO.GetComponent<RectTransform>();
        healthTextRect.anchorMin = new Vector2(0.37f, 0.02f);
        healthTextRect.anchorMax = new Vector2(1f, 0.26f);
        healthTextRect.offsetMin = Vector2.zero;
        healthTextRect.offsetMax = new Vector2(-4f, 0f);
        Text healthText = healthTextGO.GetComponent<Text>();
        healthText.font = UiFont;
        healthText.fontSize = 11;
        healthText.alignment = TextAnchor.MiddleLeft;
        healthText.color = Color.white;

        CharacterView view = root.AddComponent<CharacterView>();
        view.PortraitImage = portraitImage;
        view.NameText = nameText;
        view.HealthFillImage = healthFillImage;
        view.HealthText = healthText;
        view.TurnHighlight = highlight;

        PrefabUtility.SaveAsPrefabAsset(root, $"{PREFAB_FOLDER}/CharacterView.prefab");
        Object.DestroyImmediate(root);
    }

    private static GameObject CreateChild(Transform parent, string name, params System.Type[] components)
    {
        GameObject go = new GameObject(name, components);
        go.transform.SetParent(parent, false);
        return go;
    }

    private static void SetRect(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}
