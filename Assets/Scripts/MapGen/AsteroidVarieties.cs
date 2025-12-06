using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidVarieties : MonoBehaviour
{
    //this class exists to store the various kinds of spawnable asteroids/planets instantiated in Orbital Handlers.
    public static AsteroidVarieties i;
    [SerializeField] GameObject asteroidPrefab; //a prefab of an asteroid that can be spawned and modified

    //depending on the spawned size, sprites will be swapped out
    [SerializeField] List<Sprite> asteroidSprites;

    //getters
    public GameObject AsteroidPrefab => asteroidPrefab;
    public List<Sprite> AsteroidSprites => asteroidSprites;
    void Awake()
    {
        if (i == null)
        {
            i = this;
        }
    }

    //Asteroid names are pulled from this string array. Expanding to allow for more names to choose from.    
    public string[] asteroidNames = {"Chixculub", "Ceres", "Phobos"};
}
