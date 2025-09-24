using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static BetterPingDistance.Plugin;
using System.Linq;

namespace BetterPingDistance;

public class PingDistanceManager
{
    private TMP_FontAsset font = null!;
    private Material fontMaterial = null!;
    private Canvas pingDistanceCanvas = null!;
    private GameObject template = null!;
    private readonly Dictionary<Character, GameObject> pingDistanceInstances = new Dictionary<Character, GameObject>();
    public bool isInitialized
    {
        get
        {
            return instance != null && pingDistanceCanvas != null && template != null;
        }
    }

    public PingDistanceManager()
    {
        Log.LogInfo("PingDistanceManager has started");
    }

    public void Initialize()
    {
        if (isInitialized)
            return;

        CreateCanvas();
        CreateTemplate();

        if (isInitialized)
            Log.LogInfo($"PingDistanceManager UI initialized");
    }
    public void OnDestroy()
    {
        CleanupUI();
        if (!isInitialized)
            Log.LogInfo("PingDistanceManager UI cleaned up");
    }

    private void CleanupUI()
    {
        if (pingDistanceCanvas != null)
        {
            Object.Destroy(pingDistanceCanvas.GetComponent<CanvasScaler>());
            Object.Destroy(pingDistanceCanvas.GetComponent<GraphicRaycaster>());
            Object.Destroy(pingDistanceCanvas.gameObject);
            pingDistanceCanvas = null!;
        }
        template = null!;
        pingDistanceInstances.Clear();
    }

    private void FindFont()
    {
        TextMeshProUGUI[] textComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (TextMeshProUGUI textComponent in textComponents)
        {
            if (textComponent.name == "Text" &&
                textComponent.materialForRendering.name.Contains("DarumaDropOne-Regular SDF Outline"))
            {
                font = textComponent.font;
                fontMaterial = textComponent.materialForRendering;
                return;
            }
        }

        if (font == null)
            Log.LogError("Error finding game font");
    }

    private void CreateCanvas()
    {
        if (pingDistanceCanvas != null)
            return;

        pingDistanceInstances.Clear();

        GameObject guiManager = GameObject.Find("GAME/GUIManager");
        GameObject canvasObj = new GameObject("Canvas_BetterPingDistance", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObj.transform.SetParent(guiManager.transform);
        pingDistanceCanvas = canvasObj.GetComponent<Canvas>();
        pingDistanceCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        pingDistanceCanvas.sortingOrder = 100;
        CanvasScaler scaler = canvasObj.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
        canvasObj.layer = LayerMask.NameToLayer("UI");

        Log.LogInfo("Canvas_BetterPingDistance created");
    }

    private void CreateTemplate()
    {
        if (template != null) return;

        template = new GameObject("PingDistanceIndicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(PingDistance));
        template.transform.SetParent(pingDistanceCanvas.transform);

        GameObject distanceText = AddTMPChildren("DistanceText", template.transform, new Vector2(100f, 50f));
        distanceText.transform.SetParent(template.transform, true);

        template.SetActive(false);

        Log.LogInfo("PingDistanceIndicator template created");
    }

    private GameObject AddTMPChildren(string name, Transform parent, Vector2 size)
    {
        if (font == null)
            FindFont();

        GameObject text = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI), typeof(Shadow));
        text.transform.SetParent(parent, true);

        RectTransform textRect = text.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.sizeDelta = size;

        var textTMP = text.GetComponent<TextMeshProUGUI>();
        textTMP.font = font;
        textTMP.fontMaterial = fontMaterial;
        textTMP.fontSize = 30f;
        textTMP.color = Color.white;
        textTMP.outlineWidth = 0.1f;
        textTMP.outlineColor = Color.black;
        textTMP.enableAutoSizing = false;
        textTMP.alignment = TextAlignmentOptions.Center;
        textTMP.horizontalAlignment = HorizontalAlignmentOptions.Center;
        textTMP.verticalAlignment = VerticalAlignmentOptions.Middle;


        Shadow shadow = text.GetComponent<Shadow>();
        shadow.effectColor = new Color(0f, 0f, 0f, 0.95f);
        shadow.effectDistance = new Vector2(2f, -2f);

        return text;
    }

    public void CreateInstance(Character character, Vector3 position)
    {
        if (pingDistanceCanvas == null || template == null)
            return;

        if (pingDistanceInstances.TryGetValue(character, out var oldInstance) && oldInstance != null)
        {
            Object.Destroy(oldInstance);
        }

        GameObject newInstance = Object.Instantiate(template, pingDistanceCanvas.transform);

        var pingDistanceComponent = newInstance.GetComponent<PingDistance>();
        pingDistanceComponent.color = character.refs.customization.PlayerColor;
        pingDistanceComponent.position = position;

        newInstance.SetActive(true);
        pingDistanceInstances[character] = newInstance;
        Object.Destroy(newInstance, 1.25f);
    }
}
