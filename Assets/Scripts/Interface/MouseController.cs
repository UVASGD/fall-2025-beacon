using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class MouseController : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("TMP text used to display hovered info")]
    public TMP_Text infoDisplayText;
    public RectTransform textRoot;
    public RectTransform infoDisplayBackground;

    [Tooltip("The UI canvas that contains the infoDisplayText")]
    public Canvas canvas;

    [Header("Settings")]
    [Tooltip("Optional: LayerMask for world hoverable objects")]
    public LayerMask worldHoverMask = ~0;

    private Camera mainCamera;
    private InfoToDisplayController currentInfo;

    void Start()
    {
        mainCamera = Camera.main;
        infoDisplayText.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdateMouseFollow();
        CheckHoveredObject();
        Vector2 padding = new Vector2(12f, 8f);
        infoDisplayBackground.sizeDelta = infoDisplayText.rectTransform.sizeDelta + padding;
    }

    /// <summary>
    /// Makes the tooltip follow the mouse cursor with an offset, clamped to screen edges.
    /// </summary>
    void UpdateMouseFollow()
    {
        if (!infoDisplayText.gameObject.activeSelf) return;

        RectTransform canvasRect = canvas.transform as RectTransform;

        // Convert mouse position to local position in canvas space
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            canvas.worldCamera,
            out Vector2 localPoint);

        // Apply offset (to the right and slightly down)
        Vector2 finalPos = localPoint;

        // Optional: Clamp so tooltip stays on screen
        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 textSize = infoDisplayText.GetComponent<RectTransform>().sizeDelta;

        finalPos.x = Mathf.Clamp(finalPos.x, -canvasSize.x / 2 + textSize.x / 2, canvasSize.x / 2 - textSize.x / 2);
        finalPos.y = Mathf.Clamp(finalPos.y, -canvasSize.y / 2 + textSize.y / 2, canvasSize.y / 2 - textSize.y / 2);

        textRoot.anchoredPosition = finalPos;
    }

    /// <summary>
    /// Checks what object (UI or world) the mouse is hovering over.
    /// </summary>
    void CheckHoveredObject()
    {
        InfoToDisplayController newInfo = null;

        // 1️⃣ Check UI elements first
        if (EventSystem.current.IsPointerOverGameObject())
        {
            var pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var hit in raycastResults)
            {
                newInfo = hit.gameObject.GetComponent<InfoToDisplayController>();
                if (newInfo != null)
                    break;
            }
        }

        // 2️⃣ Check world objects (3D raycast)
        if (newInfo == null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, worldHoverMask))
            {
                newInfo = hit.collider.GetComponent<InfoToDisplayController>();
            }

            // 3️⃣ Optional fallback if collider doesn’t match visuals
            if (newInfo == null)
            {
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                if (plane.Raycast(ray, out float dist))
                {
                    Vector3 point = ray.GetPoint(dist);

                    foreach (var rend in FindObjectsOfType<Renderer>())
                    {
                        if (rend.bounds.Contains(point))
                        {
                            var info = rend.GetComponent<InfoToDisplayController>();
                            if (info != null)
                            {
                                newInfo = info;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // 4️⃣ Update tooltip visibility
        currentInfo = newInfo;

        if (currentInfo != null)
        {
            UpdateMouseFollow();
            infoDisplayText.text = currentInfo.infoText;
            infoDisplayText.gameObject.SetActive(true);
            infoDisplayBackground.gameObject.SetActive(true);
        }
        else
        {
            infoDisplayText.gameObject.SetActive(false);
            infoDisplayBackground.gameObject.SetActive(false);
        }
    }
}