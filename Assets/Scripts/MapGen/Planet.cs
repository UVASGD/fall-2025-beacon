using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Planet", menuName = "ScriptableObjects/Planet/New Planet")]
public class Planet : ScriptableObject
{
    [SerializeField] List<Sprite> basePlanetSprites; //planet sprite could change depending on damage?
    [SerializeField] int maxPlanetHealth;
    public List<Sprite> BasePlanetSprites => basePlanetSprites;
    public int MaxPlanetHealth => maxPlanetHealth;
}