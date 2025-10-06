using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private List<Building> buildingsInShop;

    private List<GameObject> previousButtons;

    private void Start()
    {
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
    }

    void OnWaveEnd()
    {
        shopObject.SetActive(true);
        buildingsInShop = GenerateBuildingsForShop();
        GenerateShopButtons(buildingsInShop);
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

    void BuyBuilding(int index)
    {
        if (playerMoney.GetMoney() < buildingsInShop[index].moneyCost)
            return;
        playerMoney.ChangeMoney(-buildingsInShop[index].moneyCost);
        playerBuildings.AddBuilding(buildingsInShop[index]);
        buildingsInShop.RemoveAt(index);
        GenerateShopButtons(buildingsInShop);
    }
}