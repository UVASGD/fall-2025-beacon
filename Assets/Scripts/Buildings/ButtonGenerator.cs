using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGenerator : MonoBehaviour
{
    [Header("UI Prefab & Container")]
    [Tooltip("Scene object or prefab with Button + Image + TMP_Text.")]
    public GameObject buttonTemplatePrefab;

    [Tooltip("Parent inside a Canvas (e.g., a VerticalLayoutGroup). If not set, we'll use the template's parent.")]
    public RectTransform buttonContainer;

    [Header("Gameplay")]
    public PlayerPlacing playerPlacing;

    private readonly List<GameObject> spawned = new();

    public void GenerateBuildingButtons(List<Building> buildings)
    {
        if (buttonTemplatePrefab == null)
        {
            Debug.LogError("[BuildingButtonGenerator] buttonTemplatePrefab not assigned.");
            return;
        }

        // Pick a container: explicit first, else template's parent (handles scene-object templates)
        var container = buttonContainer != null
            ? buttonContainer
            : buttonTemplatePrefab.transform.parent as RectTransform;

        if (container == null)
        {
            Debug.LogError("[BuildingButtonGenerator] No container set and template has no parent RectTransform. Assign buttonContainer.");
            return;
        }

        // Ensure container is under a Canvas so results are visible
        if (container.GetComponentInParent<Canvas>() == null)
            Debug.LogWarning("[BuildingButtonGenerator] buttonContainer is not under a Canvas—UI won't render.");

        ClearButtons(keepTemplate: true);

        if (buildings == null || buildings.Count == 0)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
            return;
        }

        for (int i = 0; i < buildings.Count; i++)
        {
            var b = buildings[i];

            // Instantiate as child of container (keeps anchors/layout sane)
            var go = Instantiate(buttonTemplatePrefab, container);
            go.name = $"BuildingButton_{i}_{b.name}";
            go.SetActive(true); // important if template is disabled

            // Basic RectTransform hygiene (safe with LayoutGroups)
            if (go.TryGetComponent<RectTransform>(out var rt))
            {
                rt.localScale = Vector3.one;
                rt.anchoredPosition3D = Vector3.zero;
            }

            // Label
            var label = go.GetComponentInChildren<TMP_Text>(true);
            if (label != null) label.text = b.name;
            else Debug.LogWarning($"[BBG] No TMP_Text on {go.name}");

            // Click
            if (go.TryGetComponent<Button>(out var btn))
            {
                btn.onClick.RemoveAllListeners();
                int idx = i;
                btn.onClick.AddListener(() => playerPlacing.SetSelectedIndex(idx));
            }
            else
            {
                Debug.LogWarning($"[BBG] No Button component on {go.name}");
            }

            spawned.Add(go);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void ClearButtons(bool keepTemplate = true)
    {
        foreach (var go in spawned)
            if (go != null) Destroy(go);
        spawned.Clear();

        if (!keepTemplate && buttonTemplatePrefab != null)
            buttonTemplatePrefab.SetActive(false); // usually keep template hidden
    }
}
