using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCarrierController : MonoBehaviour
{
    public float aggroRadius = 10f; //Range at which it no longer flies to Earth and targets nearest building/fleet
    public float engageRadius = 2f; //Range at which it stops moving and fires at target, actually handled by EnemyWeaponController
    public float moveSpeed = 5f;
    public LayerMask targetableLayers;

    public float respawnCooldown = 10f;
    public GameObject unitToSpawn;

    [SerializeField]
    private Transform target;

    private void Awake()
    {
        WaveManager.Singleton.OnEnemySpawn();
    }

    private void Start()
    {
        StartCoroutine(UpdateTargetCoroutine());
        StartCoroutine(SpawnUnits());
    }

    private void FixedUpdate()
    {
        if (target == null)
            UpdateTarget();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float actualEngageRadius = engageRadius;
        if (target == HomePlanetController.Singleton.transform) {
            //Continue flying on outskirts
        }
        else
        {
            //Enemy in engagement range, freeze
        }
    }

    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator SpawnUnits()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnCooldown);
            Instantiate(unitToSpawn, transform.position, transform.rotation);
        }
    }

    void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius, targetableLayers);
        if (hits.Length == 0)
        {
            target = HomePlanetController.Singleton.transform;
        }
        else
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            foreach (Collider collider in hits)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance > aggroRadius) { continue; }
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
            if (closestCollider != null)
            {
                target = closestCollider.transform;
            }
            else
            {
                target = HomePlanetController.Singleton.transform;
            }
        }
    }

    private void OnDestroy()
    {
        WaveManager.Singleton.OnEnemyDie();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
    }
}
