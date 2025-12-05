using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionManager : MonoBehaviour
{
    public static FactionManager i;
    [SerializeField] List<FactionBase> factionBases;
    private List<Faction> factions;
    private int playerFactionID = -1;

    private Faction playerFaction;
    private List<Faction> enemyAIFactions;

    void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(this); //move to DontDestroyOnLoad for usage in the game scene (as this is originally spawned in the title scene)

            //in order to accomodate in-engine testing from the gameplay scene, another object will check whether or not an instance of factionmanager exists on awake.
            //NOTE: in order for that method to work, make sure this script runs AHEAD of the factionManagerSpawner in Execution Order!

            InitializeFactions();
        }
        else
        {
            Destroy(this);
        }

        FactionSelectionIcon.onFactionSelectionClick += SetPlayerFactionIndex;
    }
    private void SetPlayerFactionIndex(int index)
    {
        if (index != -1)
            playerFactionID = index;
    }
    private void InitializeFactions()
    {
        factions = new List<Faction>();
        enemyAIFactions = new List<Faction>();

        //constructing each faction from its appropriate base
        foreach (var faction in factionBases)
        {
            factions.Add(new Faction(faction));
        }

        Debug.Log("Factions initialized");
    }

    public void PrepGameplayFactions()
    {
        //split factions list between the player and enemy AI for usage in loading in the gameplay scene
        for (int i = 0; i < factions.Count; i++)
        {
            if (i == playerFactionID)
            {
                playerFaction = factions[i];
            }
            else
            {
                enemyAIFactions.Add(factions[i]);
            }
        }
    }

    public List<Faction> ReturnFactionList()
    {
        return factions;
    }

    public Faction GetPlayerFaction()
    {
        return playerFaction;
    }

    public int GetPlayerFactionID()
    {
        return playerFactionID;
    }
}