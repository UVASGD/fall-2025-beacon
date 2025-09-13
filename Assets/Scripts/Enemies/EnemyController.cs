using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class EnemyController : MonoBehaviour
{
    public float aggroRadius = 10f; //Range at which it no longer flies to Earth and targets nearest building/fleet
    public float engageRadius = 2f; //Range at which it stops moving and fires at target, actually handled by EnemyWeaponController
    public float moveSpeed = 5f;
    public LayerMask targetableLayers;

    private SwarmController swarmController;
    private WeaponController weaponController;  
    [SerializeField]
    private Transform target;

    private void Awake()
    {
        WaveManager.Singleton.OnEnemySpawn();
        swarmController = GetComponent<SwarmController>();
        weaponController = GetComponent<WeaponController>();
    }

    private void Start()
    {
        StartCoroutine(UpdateTargetCoroutine());
    }

    private void FixedUpdate()
    {
        if (target == null)
            UpdateTarget();

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        float actualEngageRadius = engageRadius;
        if (target == HomePlanetController.Singleton.transform) { actualEngageRadius += 5; }

        if(distanceToTarget < actualEngageRadius)
        {
            swarmController.isMoving = false;
            weaponController.SetEnableShooting(true);
        }
        else
        {
            swarmController.isMoving = true;
            Vector3 desiredMoveDirection = (target.position - transform.position); desiredMoveDirection.y = 0;
            swarmController.moveDirection = desiredMoveDirection;
            transform.position += desiredMoveDirection.normalized * moveSpeed * Time.deltaTime;
            weaponController.SetEnableShooting(false);
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

    void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, aggroRadius, targetableLayers);
        if(hits.Length == 0)
        {
            target = HomePlanetController.Singleton.transform;
        }
        else
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            foreach(Collider collider in hits)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if(distance > aggroRadius) { continue; }
                if(distance < closestDistance)
                {
                    closestDistance = distance;
                    closestCollider = collider;
                }
            }
            if(closestCollider != null)
            {
                target = closestCollider.transform;
            }
            else
            {
                target = HomePlanetController.Singleton.transform;
            }
        }
        weaponController.target = target;
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
