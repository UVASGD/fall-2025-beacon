using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [Header("Wiring")]
    public RectTransform panel;           // assign your TooltipPanel (RectTransform)
    public TextMeshProUGUI text;          // assign the TMP text inside the panel
    public Vector2 offset = new Vector2(24f, -24f);

    [Header("Options")]
    public bool debugMode = false;
    [Tooltip("Seconds to wait after exit before hiding; prevents flicker.")]
    public float hideDelay = 0.15f;

    Canvas _rootCanvas;
    CanvasGroup _cg;
    float _hideAt = -1f;
    bool _visible;
    int _hoverRefs = 0;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // Root canvas
        _rootCanvas = GetComponentInParent<Canvas>();
        if (_rootCanvas == null) Debug.LogWarning("[Tooltip] No Canvas found in parents.");

        // Guard against missing wiring
        if (panel == null)
        {
            Debug.LogError("[Tooltip] 'panel' is not assigned. Assign your TooltipPanel RectTransform in the Inspector.");
            return; // bail out safely; no component access
        }

        // Ensure CanvasGroup exists and is non-interactive
        _cg = panel.GetComponent<CanvasGroup>();
        if (_cg == null) _cg = panel.gameObject.AddComponent<CanvasGroup>();
        _cg.blocksRaycasts = false;
        _cg.interactable = false;

        // Ensure the panel/text don't intercept raycasts
        var bg = panel.GetComponent<Image>();
        if (bg != null) bg.raycastTarget = false;
        if (text != null) text.raycastTarget = false;
        else Debug.LogWarning("[Tooltip] 'text' is not assigned. Assign the TMP_Text inside the panel.");

        HideImmediate();
    }

    void LateUpdate()
    {
        if (_visible && panel != null && panel.gameObject.activeSelf && _rootCanvas != null)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _rootCanvas.transform as RectTransform,
                Input.mousePosition,
                _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera,
                out var localPos);
            panel.anchoredPosition = localPos + offset;
        }

        if (_hideAt > 0f && Time.unscaledTime >= _hideAt)
            HideImmediate();
    }

    public void Show(string richText)
    {
        if (panel == null) return;
        _hideAt = -1f;
        if (text != null) text.text = richText;
        panel.SetAsLastSibling();               // ensure on top
        panel.gameObject.SetActive(true);
        _visible = true;
    }

    public void ScheduleHide()
    {
        if (!_visible) return;
        _hideAt = Time.unscaledTime + Mathf.Max(0f, hideDelay);
    }

    public void HideImmediate()
    {
        _hideAt = -1f;
        _visible = false;
        _hoverRefs = 0;
        if (panel != null) panel.gameObject.SetActive(false);
    }

    // Hover reference counting so UI churn can’t strand the tooltip
    public void RegisterHover() { _hoverRefs++; _hideAt = -1f; }
    public void UnregisterHover() { _hoverRefs = Mathf.Max(0, _hoverRefs - 1); if (_hoverRefs == 0) ScheduleHide(); }

    public void SetDebugMode(bool on) => debugMode = on;
}
