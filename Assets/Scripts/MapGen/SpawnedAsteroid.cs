using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnedAsteroid : MonoBehaviour
{
    //data that contains information about a spawned-in asteroid
    [SerializeField] OrbitalData orbitalData;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] BoxCollider buildableArea;
    [SerializeField] SpriteRenderer buildableAreaRenderer;
    [SerializeField] CelestialBodyData bodyData;
    public OrbitalData OrbitalData => orbitalData;

    public const int DEFAULTORECONTENT = 40;

    public void InitializeAsteroid(float asteroidScale)
    {
        int spriteIndex = 0;
        switch (asteroidScale) //assign the sprite of the captured body based on its scale
        {
            case < 0.75f:
                spriteIndex = 0;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(100);
                break;
            case < 1.25f:
                spriteIndex = 1;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(150);
                break;
            case < 2.5f:
                spriteIndex = 2;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(200);
                break;
            case < 3.5f:
                spriteIndex = 3;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(250);
                break;
            case < 4.5f:
                spriteIndex = 4;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(300);
                break;
            default:
                spriteIndex = 5;
                orbitalData.SetBaseOreContent(DEFAULTORECONTENT);
                orbitalData.SetPlanetMaxHealth(350);
                break;
        }
        mainRenderer.sprite = AsteroidVarieties.i.AsteroidSprites[spriteIndex];
        orbitalData.SetBuildingSize(spriteIndex);

        //set the size of the buildableArea and its renderer
        buildableArea.size = Vector3.one * Mathf.Max(1, spriteIndex) * 4;
        buildableAreaRenderer.gameObject.transform.localScale = Vector3.one * spriteIndex;
    }
}
