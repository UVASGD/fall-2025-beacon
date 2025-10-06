using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Wiring (assign on the prefab)")]
    public Image background;            // root background (tinted by spec.backgroundColor)
    public Image icon;                  // optional icon image
    public TMP_Text titleLabel;         // large text
    public TMP_Text subtitleLabel;      // small text under/next to title
    public TMP_Text badgeLabel;         // corner badge text (e.g., cost)

    [Header("Styles")]
    public Color disabledTint = new Color(1, 1, 1, 0.35f);

    Button _button;
    UIButtonSpec _spec;

    void Awake()
    {
        _button = GetComponent<Button>();
        if (background == null) background = GetComponent<Image>();
    }

    public void Bind(UIButtonSpec spec)
    {
        _spec = spec;

        if (titleLabel != null) titleLabel.text = spec.title ?? "";
        if (subtitleLabel != null) subtitleLabel.text = spec.subtitle ?? "";
        if (badgeLabel != null) badgeLabel.text = string.IsNullOrEmpty(spec.badgeText) ? "" : spec.badgeText;

        if (icon != null)
        {
            icon.enabled = (spec.icon != null);
            icon.sprite = spec.icon;
        }

        if (background != null)
            background.color = spec.backgroundColor.a > 0 ? spec.backgroundColor : Color.white;

        // Interactable/disabled visuals
        _button.interactable = spec.interactable;
        var cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        cg.alpha = spec.interactable ? 1f : 0.6f;

        // Click
        _button.onClick.RemoveAllListeners();
        if (spec.onClick != null) _button.onClick.AddListener(() => spec.onClick.Invoke());
    }

    // Tooltip integration (works with the Tooltip singleton we built earlier)
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(_spec.tooltipRichText) || Tooltip.Instance == null) return;
        Tooltip.Instance.RegisterHover();
        Tooltip.Instance.Show(_spec.tooltipRichText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Tooltip.Instance == null) return;
        Tooltip.Instance.UnregisterHover();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Tooltip.Instance == null) return;
        Tooltip.Instance.HideImmediate();
    }

    void OnDisable()
    {
        if (Tooltip.Instance != null) Tooltip.Instance.UnregisterHover();
    }
}
