using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingButtonController : MonoBehaviour
{
    public Image buildingImage;
    public Image turretImage;
    public TMP_Text nameText;

    public void DisplayBuilding(Building _building)
    {
        buildingImage.sprite = _building.buildingSprite;
        nameText.text = _building.name;
        if (_building.turretSprite != null)
        {
            turretImage.enabled = true;
            turretImage.sprite = _building.turretSprite;
        }
        else
        {
            turretImage.enabled = false;
        }
    }
}
