using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerController : MonoBehaviour
{
    public GameObject swarmPrefab;
    public GameObject beaconPrefab;

    private GameObject connectedSwarm;
    private GameObject connectedBeacon;

    private float swarmHealth = 100f;
    private float swarmFireRate = 100f;
    private float swarmSpeed = 5f;

    public LevelController levelController; 
    public DamageController damageController;

    private void Awake()
    {
        WaveManager.Singleton.onWaveStart += OnWaveStart;
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
        levelController = GetComponent<LevelController>();  
        levelController.onLevelUp += OnLevelUp;

        swarmHealth = swarmPrefab.GetComponent<PlayerSwarmHealth>().GetMaxHealth();
        damageController.damage = swarmPrefab.GetComponent<WeaponController>().damage;
        swarmFireRate = swarmPrefab.GetComponent<WeaponController>().fireRate;
        swarmSpeed = swarmPrefab.GetComponent<HumanController>().speed;
    }

    private void Start()
    {
        GameObject swarm = Instantiate(swarmPrefab, transform.position, transform.rotation);
        GameObject beacon = Instantiate(beaconPrefab, transform.position, transform.rotation);
        swarm.GetComponent<HumanController>().connectedBeacon = beacon.transform;
        InitializeSwarmStats(swarm);
        connectedSwarm = swarm;
        connectedBeacon = beacon;
    }

    void OnWaveEnd()
    {
        Destroy(connectedSwarm);
        GameObject swarm = Instantiate(swarmPrefab, connectedBeacon.transform.position, transform.rotation);
        swarm.GetComponent<HumanController>().connectedBeacon = connectedBeacon.transform;
        InitializeSwarmStats(swarm);
        connectedSwarm = swarm;
    }

    void OnWaveStart()
    {
        Destroy(connectedSwarm);
        GameObject swarm = Instantiate(swarmPrefab, connectedBeacon.transform.position, transform.rotation);
        swarm.GetComponent<HumanController>().connectedBeacon = connectedBeacon.transform;
        InitializeSwarmStats(swarm);
        connectedSwarm = swarm;
    }

    private void InitializeSwarmStats(GameObject swarm)
    {
        swarm.GetComponent<PlayerSwarmHealth>().InitializeHealth(swarmHealth);
        swarm.GetComponent<WeaponController>().damage = damageController.damage;
        swarm.GetComponent<WeaponController>().fireRate = swarmFireRate;
        swarm.GetComponent <HumanController>().speed = swarmSpeed;
    }

    private void OnLevelUp()
    {
        int currentLevel = levelController.GetLevel();
        swarmHealth *= 1f + (1f / (currentLevel - 1));
        damageController.damage *= 1f + (1f / (currentLevel - 1));
        swarmFireRate *= 1f + (1f / (currentLevel - 1));
        swarmSpeed *= 1f + (1f / (currentLevel - 1));
    }

    private void OnDestroy()
    {
        if(WaveManager.Singleton != null)
        {
            WaveManager.Singleton.onWaveStart -= OnWaveStart;
            WaveManager.Singleton.onWaveFinished -= OnWaveEnd;
        }
    }
}
