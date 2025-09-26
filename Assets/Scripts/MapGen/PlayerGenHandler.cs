using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGenHandler : MonoBehaviour
{
    [SerializeField] HomePlanetController homePlanet;
    [SerializeField] ShopManager shopManager;
    [SerializeField] PlayerBuildings playerBuildings;
    void Awake()
    {
        if (FactionManager.i != null)
        {
            //if the faction manager exists
            SetPlayerState();
        }
    }

    private void SetPlayerState()
    {
        Faction player = FactionManager.i.GetPlayerFaction(); //reference assigned here to reduce function calls in this method

        homePlanet.SetHomePlanet(player.FactionBase.HomePlanet); //set the central planet to the player faction
        shopManager.possibleBuildings = player.FactionBase.FactionBuildings; //set the appropriate faction buildings
        playerBuildings.SetInitialBuildings(player.FactionBase.StartingBuildings); //sets the free initial buildings before wave 1
    }
}