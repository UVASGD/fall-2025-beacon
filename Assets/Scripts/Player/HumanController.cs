using System.Collections;
using UnityEngine;

public class HumanController : MonoBehaviour
{
    public Transform connectedBeacon;
    public float speed = 5f;
    public float rangeRadius = 5f;
    public LayerMask enemyLayers;

    private SwarmController swarmController;
    private WeaponController weaponController;

    [SerializeField]
    private Transform target;

    private void Awake()
    {
        swarmController = GetComponent<SwarmController>();
        weaponController = GetComponent<WeaponController>();
        StartCoroutine(UpdateTargetCoroutine());
    }

    private void Update()
    {
        Vector3 beaconPosition = connectedBeacon.position; beaconPosition.y = 0;
        Vector3 thisPosition = transform.position; thisPosition.y = 0;
        float distanceToBeacon = Vector3.Distance(thisPosition, beaconPosition);
        if(distanceToBeacon > 0 )
        {
            Vector3 desiredMoveDirection = (beaconPosition - thisPosition).normalized;
            swarmController.isMoving = true;
            swarmController.moveDirection = desiredMoveDirection;
            transform.position += desiredMoveDirection * Mathf.Min(speed * Time.deltaTime, distanceToBeacon);
            weaponController.SetEnableShooting(true);
        }
        else
        {
            if(swarmController.isMoving)
            {
                UpdateTarget();
                swarmController.isMoving = false;
            }
            weaponController.target = target;
            weaponController.SetEnableShooting(true);
        }
    }

    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds(0.25f);
        }
    }

    void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, rangeRadius, enemyLayers);
        if (hits.Length == 0)
        {
            target = null;
        }
        else
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            foreach (Collider collider in hits)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance > rangeRadius) { continue; }
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
                target = null;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rangeRadius);
    }
}
