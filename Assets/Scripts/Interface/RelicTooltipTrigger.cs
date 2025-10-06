using UnityEngine;
using UnityEngine.EventSystems;

public class RelicTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public RelicInstance relic;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Tooltip.Instance == null || relic == null) return;
        Tooltip.Instance.RegisterHover();
        Tooltip.Instance.Show(RelicUI.BuildDescription(relic, Tooltip.Instance.debugMode));
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
