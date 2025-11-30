using System.Collections;
using UnityEngine;

public class TurretTargetingController : MonoBehaviour
{
    public float rangeRadius = 20f;
    public LayerMask enemyLayers;

    public CannonController cannonController;

    [SerializeField]
    private Transform target;

    private void Start()
    {
        StartCoroutine(UpdateTargetCoroutine());
    }

    public Transform GetTarget() { return target; } 

    IEnumerator UpdateTargetCoroutine()
    {
        while (true)
        {
            UpdateTarget();
            yield return new WaitForSeconds((1/cannonController.fireRate) * 0.9f * GlobalSettings.i.TimeScale);
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
