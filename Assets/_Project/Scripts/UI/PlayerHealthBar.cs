using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    private const string CanvasName = "PlayerHealthBarCanvas";

    [SerializeField] private Image remainingHealthImage;
    [SerializeField] private Image lostHealthImage;
    [SerializeField] private PlayerController target;

    private RectTransform remainingHealthRectTransform;

    public void Initialize(PlayerController player, Image remainingImage, Image lostImage)
    {
        remainingHealthImage = remainingImage;
        lostHealthImage = lostImage;
        remainingHealthRectTransform = remainingHealthImage.rectTransform;
        SetTarget(player);
    }

    private void OnEnable()
    {
        if (remainingHealthImage != null)
        {
            remainingHealthRectTransform = remainingHealthImage.rectTransform;
        }

        if (target == null)
        {
            target = FindPlayer();
        }

        Subscribe();
        Refresh();
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void Update()
    {
        if (target == null)
        {
            SetTarget(FindPlayer());
        }
    }

    private void SetTarget(PlayerController player)
    {
        if (target == player)
        {
            Refresh();
            return;
        }

        Unsubscribe();
        target = player;
        Subscribe();
        Refresh();
    }

    private void Subscribe()
    {
        if (target != null)
        {
            target.HealthChanged += HandleHealthChanged;
        }
    }

    private void Unsubscribe()
    {
        if (target != null)
        {
            target.HealthChanged -= HandleHealthChanged;
        }
    }

    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        Refresh();
    }

    private void Refresh()
    {
        float ratio = target == null ? 1f : Mathf.Clamp01(target.HealthRatio);

        if (remainingHealthRectTransform != null)
        {
            remainingHealthRectTransform.anchorMax = new Vector2(1f, ratio);
        }

        if (remainingHealthImage != null)
        {
            remainingHealthImage.enabled = target != null && ratio > 0f;
        }

        if (lostHealthImage != null)
        {
            lostHealthImage.enabled = target != null;
        }
    }

    internal static PlayerController FindPlayer()
    {
#if UNITY_2023_1_OR_NEWER
        return FindFirstObjectByType<PlayerController>();
#else
        return FindObjectOfType<PlayerController>();
#endif
    }

    internal static bool HasHealthBar()
    {
        return GameObject.Find(CanvasName) != null;
    }

    internal static void Create(PlayerController player)
    {
        if (player == null || HasHealthBar())
        {
            return;
        }

        GameObject canvasObject = new GameObject(CanvasName, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler canvasScaler = canvasObject.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;

        GameObject gaugeObject = new GameObject("Player HP Gauge", typeof(RectTransform), typeof(PlayerHealthBar));
        gaugeObject.transform.SetParent(canvasObject.transform, false);

        RectTransform gaugeRectTransform = gaugeObject.GetComponent<RectTransform>();
        gaugeRectTransform.anchorMin = new Vector2(0f, 0.5f);
        gaugeRectTransform.anchorMax = new Vector2(0f, 0.5f);
        gaugeRectTransform.pivot = new Vector2(0f, 0.5f);
        gaugeRectTransform.anchoredPosition = new Vector2(24f, 0f);
        gaugeRectTransform.sizeDelta = new Vector2(20f, 220f);

        Image lostImage = CreateGaugeImage("Lost HP", gaugeRectTransform, new Color(0.86f, 0.08f, 0.08f, 1f));
        Image remainingImage = CreateGaugeImage("Remaining HP", gaugeRectTransform, new Color(0.08f, 0.38f, 1f, 1f));
        RectTransform remainingRectTransform = remainingImage.rectTransform;
        remainingRectTransform.anchorMin = Vector2.zero;
        remainingRectTransform.anchorMax = Vector2.one;
        remainingRectTransform.pivot = new Vector2(0.5f, 0f);

        PlayerHealthBar healthBar = gaugeObject.GetComponent<PlayerHealthBar>();
        healthBar.Initialize(player, remainingImage, lostImage);
    }

    private static Image CreateGaugeImage(string name, RectTransform parent, Color color)
    {
        GameObject imageObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        imageObject.transform.SetParent(parent, false);

        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        Image image = imageObject.GetComponent<Image>();
        image.color = color;
        image.raycastTarget = false;

        return image;
    }
}

public static class PlayerHealthBarBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Initialize()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
        CreateForCurrentScene();
    }

    private static void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CreateForCurrentScene();
    }

    private static void CreateForCurrentScene()
    {
        PlayerHealthBar.Create(PlayerHealthBar.FindPlayer());
    }
}
