using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonGenerator : MonoBehaviour
{
    [Header("Template & Container")]
    public UIButtonView template;          // assign your button prefab (scene object or prefab)
    public RectTransform container;        // parent under a Canvas (e.g., a VerticalLayoutGroup)

    [Header("Pooling")]
    public int preload = 0;                // optional: pre-instantiate N
    public bool keepInactiveTemplate = true;

    readonly List<UIButtonView> _pool = new();
    readonly List<UIButtonView> _active = new();

    void Awake()
    {
        if (template == null) { Debug.LogError("[UIButtonGenerator] Template not assigned."); return; }
        if (container == null) container = template.transform.parent as RectTransform;

        if (preload > 0) EnsurePool(preload);
        if (keepInactiveTemplate && template.gameObject.activeSelf) template.gameObject.SetActive(false);
    }

    void EnsurePool(int count)
    {
        while (_pool.Count < count)
        {
            var v = Instantiate(template, container);
            v.gameObject.SetActive(false);
            _pool.Add(v);
        }
    }

    UIButtonView GetView()
    {
        if (_pool.Count > 0)
        {
            var v = _pool[_pool.Count - 1];
            _pool.RemoveAt(_pool.Count - 1);
            return v;
        }
        return Instantiate(template, container);
    }

    public void Clear()
    {
        foreach (var v in _active)
        {
            if (v != null) { v.gameObject.SetActive(false); _pool.Add(v); }
        }
        _active.Clear();

        if (container != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void Populate(IReadOnlyList<UIButtonSpec> specs)
    {
        if (template == null || container == null) return;

        Clear();
        EnsurePool(specs.Count);

        for (int i = 0; i < specs.Count; i++)
        {
            var view = GetView();
            view.transform.SetParent(container, false);
            view.gameObject.name = $"Button_{i}_{specs[i].title}";
            view.gameObject.SetActive(true);
            view.Bind(specs[i]);
            _active.Add(view);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }
}
