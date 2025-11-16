using System.Collections.Generic;
using UnityEngine;
using TMPro; // Assuming TMM_Text is similar to TMP_Text
using UnityEngine.UI;

public class BuildingButtonGenerator : MonoBehaviour
{
    public GameObject buttonTemplatePrefab;   // Prefab to clone
    public PlayerPlacing playerPlacing;

    private List<GameObject> previousButtons;
    public void GenerateBuildingButtons(List<Building> buildings)
    {
        if(previousButtons != null) { 
            for(int i = 0; i < previousButtons.Count; i++)
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
            buttonGO.GetComponent<BuildingButtonController>().DisplayBuilding(building);
            int indexToSelect = currentIndex;
            buttonGO.GetComponent<Button>().onClick.AddListener(() => playerPlacing.SetSelectedIndex(indexToSelect));
            currentIndex++;
        }
    }
}
