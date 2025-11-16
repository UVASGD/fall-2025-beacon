using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "ScriptableObjects/Building", order = 1)]
public class Building : ScriptableObject
{
    public GameObject buildingPrefab;
    public int moneyCost = 30;
    public string description = "Empty building description";
    public Sprite buildingSprite;
    public Sprite turretSprite;
}
