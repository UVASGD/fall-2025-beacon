using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public GameObject shopObject;
    public List<Building> possibleBuildings;
    public int buildingsInShopCount = 3;
    public GameObject buttonTemplatePrefab;
    public PlayerBuildings playerBuildings;
    public PlayerMoney playerMoney;
    public RelicManager relics;
    private bool showingRelics = false;

    // UPDATED: now uses RelicInstance
    private List<RelicInstance> relicChoices = new List<RelicInstance>();
    private bool _subscribedToWaves = false;
    public TMP_Text headerLabel;

    [SerializeField]
    private List<Building> buildingsInShop;
    private List<GameObject> previousButtons = new List<GameObject>();

    private void Start()
    {
        if (WaveManager.Singleton != null)
            WaveManager.Singleton.onWaveFinished += HandleWaveFinished_OpenShop;
    }

    private void OnDisable()
    {
        if (WaveManager.Singleton != null)
            WaveManager.Singleton.onWaveFinished -= HandleWaveFinished_OpenShop;
    }

    private void TrySubscribeToWaveEvents()
    {
        var wm = WaveManager.Singleton ?? FindObjectOfType<WaveManager>();
        if (wm != null && !_subscribedToWaves)
        {
            wm.onWaveFinished += HandleWaveFinished_OpenShop;
            _subscribedToWaves = true;
        }
    }

    void OnWaveEnd()
    {
        shopObject.SetActive(true);
        buildingsInShop = GenerateBuildingsForShop();
        GenerateShopButtons(buildingsInShop);
    }

    private void HandleWaveFinished_OpenShop()
    {
        if (relics != null) relics.NotifyWaveFinished();

        if (relics != null && relics.ShouldOfferRelicNow())
        {
            relicChoices = relics.RollChoices(); // List<RelicInstance>
            shopObject.SetActive(true);
            showingRelics = true;
            if (headerLabel != null) headerLabel.text = "Choose a Relic";
            GenerateRelicButtons(relicChoices);
        }
        else
        {
            OpenBuildingShop();
        }
    }

    private void OpenBuildingShop()
    {
        shopObject.SetActive(true);
        showingRelics = false;
        buildingsInShop = GenerateBuildingsForShop();
        GenerateShopButtons(buildingsInShop);
        if (headerLabel != null) headerLabel.text = "Shop";
    }

    List<Building> GenerateBuildingsForShop()
    {
        List<Building> returnList = new List<Building>();
        for (int i = 0; i < buildingsInShopCount; i++)
        {
            returnList.Add(possibleBuildings[Random.Range(0, possibleBuildings.Count)]);
        }
        return returnList;
    }

    public void CloseShop()
    {
        shopObject.SetActive(false);
        if (WaveManager.Singleton != null)
            WaveManager.Singleton.ShowStartNextWaveButton();
    }

    void GenerateShopButtons(List<Building> buildings)
    {
        if (previousButtons != null)
        {
            for (int i = 0; i < previousButtons.Count; i++)
            {
                Destroy(previousButtons[i]);
            }
            previousButtons.Clear();
        }
        else
        {
            previousButtons = new List<GameObject>();
        }

        int currentIndex = 0;
        foreach (Building building in buildings)
        {
            GameObject buttonGO = Instantiate(buttonTemplatePrefab, buttonTemplatePrefab.transform.position, buttonTemplatePrefab.transform.rotation);
            buttonGO.transform.SetParent(buttonTemplatePrefab.transform.parent, false);
            buttonGO.SetActive(true);
            previousButtons.Add(buttonGO);
            TMP_Text label = buttonGO.GetComponentInChildren<TMP_Text>();
            int indexToSelect = currentIndex;
            buttonGO.GetComponent<Button>().onClick.RemoveAllListeners();
            buttonGO.GetComponent<Button>().onClick.AddListener(() => BuyBuilding(indexToSelect));
            if (label != null)
            {
                label.text = $"{building.name} ({building.moneyCost})";
            }
            else
            {
                Debug.LogWarning("No TMP_Text found on button prefab.");
            }
            currentIndex++;
        }
    }

    // UPDATED: RelicInstance version
    void GenerateRelicButtons(List<RelicInstance> relicsToShow)
    {
        foreach (var go in previousButtons) Destroy(go);
        previousButtons.Clear();

        for (int i = 0; i < relicsToShow.Count; i++)
        {
            var relic = relicsToShow[i];

            GameObject buttonGO = Instantiate(
                buttonTemplatePrefab,
                buttonTemplatePrefab.transform.position,
                buttonTemplatePrefab.transform.rotation,
                buttonTemplatePrefab.transform.parent
            );
            buttonGO.SetActive(true);
            previousButtons.Add(buttonGO);

            var tmp = buttonGO.GetComponentInChildren<TMP_Text>();
            var ugui = (tmp == null) ? buttonGO.GetComponentInChildren<Text>() : null;

            string desc = BuildRelicDescription(relic);
            string labelText = relic.displayName;
            if (!string.IsNullOrEmpty(desc))
                labelText += $"\n<size=80%>{desc}</size>";

            if (tmp != null) tmp.text = labelText;
            if (ugui != null) ugui.text = labelText;

            var img = buttonGO.GetComponentInChildren<Image>();
            if (img != null && relic.icon != null) img.sprite = relic.icon;

            var btn = buttonGO.GetComponent<Button>();
            int captured = i;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => ChooseRelic(captured));
        }
    }

    // Compact description from resolved modifiers, e.g. "Damage +10%, FireRate +8%"
    string BuildRelicDescription(RelicInstance r)
    {
        if (r == null || r.resolvedModifiers == null || r.resolvedModifiers.Count == 0) return "";
        var sb = new StringBuilder();
        for (int i = 0; i < r.resolvedModifiers.Count; i++)
        {
            var m = r.resolvedModifiers[i];
            string delta = Mathf.Abs(m.mul) > 0.0001f ? $"{m.mul * 100f:+0.#;-0.#}%" : $"{m.add:+0.##;-0.##}";
            sb.Append($"{m.stat} {delta}");
            if (i < r.resolvedModifiers.Count - 1) sb.Append(", ");
        }
        return sb.ToString();
    }

    void BuyBuilding(int index)
    {
        if (playerMoney.GetMoney() < buildingsInShop[index].moneyCost)
            return;
        playerMoney.ChangeMoney(-buildingsInShop[index].moneyCost);
        playerBuildings.AddBuilding(buildingsInShop[index]);
        buildingsInShop.RemoveAt(index);
        GenerateShopButtons(buildingsInShop);
    }

    // UPDATED: take a RelicInstance directly
    void ChooseRelic(int index)
    {
        relics.Take(relicChoices[index]); // applies stats globally

        shopObject.SetActive(false);
        if (WaveManager.Singleton != null && WaveManager.Singleton.startNextWaveButton != null)
            WaveManager.Singleton.startNextWaveButton.SetActive(true);

        showingRelics = false;
        relicChoices.Clear();
    }
}
