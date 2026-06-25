using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image remainingHealthImage;
    [SerializeField] private Image lostHealthImage;
    [SerializeField] private PlayerController target;

    private RectTransform remainingHealthRectTransform;

    private void Awake()
    {
        if (remainingHealthImage != null)
        {
            remainingHealthRectTransform = remainingHealthImage.rectTransform;
        }
    }

    private void OnEnable()
    {
        if (target == null)
        {
            target = FindAnyObjectByType<PlayerController>();
        }

        if (target != null)
        {
            target.HealthChanged += HandleHealthChanged;
        }
    }

    private void Start()
    {
        Refresh();
    }

    private void OnDisable()
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
        if (target == null)
        {
            Debug.LogWarning("PlayerHealthBar: target is not assigned.");
            return;
        }

        if (remainingHealthImage == null)
        {
            Debug.LogWarning("PlayerHealthBar: remainingHealthImage is not assigned.");
            return;
        }

        if (remainingHealthRectTransform == null)
        {
            remainingHealthRectTransform = remainingHealthImage.rectTransform;
        }

        float ratio = Mathf.Clamp01(target.HealthRatio);

        remainingHealthRectTransform.anchorMax = new Vector2(1f, ratio);

        remainingHealthImage.enabled = ratio > 0f;

        if (lostHealthImage != null)
        {
            lostHealthImage.enabled = true;
        }
    }
}