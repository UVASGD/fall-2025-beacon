using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnedAsteroid : MonoBehaviour
{
    //data that contains information about a spawned-in asteroid
    [SerializeField] OrbitalData orbitalData;
    [SerializeField] SpriteRenderer mainRenderer;
    [SerializeField] BoxCollider buildableArea;
    [SerializeField] SpriteRenderer buildableAreaRenderer;
    public OrbitalData OrbitalData => orbitalData;

    public void InitializeAsteroid(float asteroidScale)
    {
        //renderer.gameObject.transform.localScale = new Vector3(asteroidScale, 1, asteroidScale); //changing this scale also alters the size of the construction box.

        int spriteIndex = 0;
        switch (asteroidScale) //assign the sprite of the captured body based on its scale
        {
            case < 0.75f:
                spriteIndex = 0;
                orbitalData.SetBaseOreContent(Random.Range(1, 100));
                orbitalData.SetPlanetMaxHealth(150);
                break;
            case < 1.25f:
                spriteIndex = 1;
                orbitalData.SetBaseOreContent(Random.Range(50, 250));
                orbitalData.SetPlanetMaxHealth(250);
                break;
            case < 2.5f:
                spriteIndex = 2;
                orbitalData.SetBaseOreContent(Random.Range(150, 400));
                orbitalData.SetPlanetMaxHealth(350);
                break;
            case < 3.5f:
                spriteIndex = 3;
                orbitalData.SetBaseOreContent(Random.Range(250, 550));
                orbitalData.SetPlanetMaxHealth(450);
                break;
            case < 4.5f:
                spriteIndex = 4;
                orbitalData.SetBaseOreContent(Random.Range(300, 650));
                orbitalData.SetPlanetMaxHealth(550);
                break;

            default:
                spriteIndex = 5;
                orbitalData.SetBaseOreContent(Random.Range(350, 800));
                orbitalData.SetPlanetMaxHealth(650);
                break;
        }
        mainRenderer.sprite = AsteroidVarieties.i.AsteroidSprites[spriteIndex];

        //set the size of the buildableArea and its renderer
        buildableArea.size = Vector3.one * spriteIndex * 4;
        buildableAreaRenderer.gameObject.transform.localScale = Vector3.one * spriteIndex;
    }
}