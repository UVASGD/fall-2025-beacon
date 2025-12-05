using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MissileController : MonoBehaviour
{
    public float speed;
    public float rotationSpeed = 90;
    public float damage;
    public LayerMask enemyLayers;

    private Transform target;
    private float maxSearchRange = 10000f;
    private float maxTravelDistance = 100f;
    private float travelDistance;

    private void Awake()
    {
        WaveManager.Singleton.onWaveStart += OnWaveStart;
        WaveManager.Singleton.onWaveFinished += OnWaveEnd;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            UpdateTarget();
        }
        if (travelDistance > maxTravelDistance)
            Destroy(gameObject);

        if (target == null)
        {
            transform.position += transform.forward * speed * Time.fixedDeltaTime * GlobalSettings.i.TimeScale;
            travelDistance += speed * Time.fixedDeltaTime * GlobalSettings.i.TimeScale;
        }
        else
        {
            Vector3 directionToTarget = target.position - transform.position;
            float distanceToTarget = directionToTarget.magnitude;
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                Quaternion.LookRotation(directionToTarget),
                rotationSpeed * Time.fixedDeltaTime * GlobalSettings.i.TimeScale
            );
            transform.position += transform.forward * Mathf.Min(speed * Time.fixedDeltaTime * GlobalSettings.i.TimeScale, distanceToTarget);

            if (Vector3.Distance(transform.position, target.position) < 0.05f)
            {
                //Hit
                float returnDam = target.GetComponent<IHealth>().ChangeHealth(-damage);
                Destroy(gameObject);
            }
            else
            {
                travelDistance += Mathf.Min(speed * Time.fixedDeltaTime * GlobalSettings.i.TimeScale, distanceToTarget);
            }
        }
    }

    void UpdateTarget()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, maxSearchRange, enemyLayers);
        
        if (hits.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            Collider closestCollider = null;
            foreach (Collider collider in hits)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance > maxSearchRange) { continue; }
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
        else
        {
            target = null;
        }
    }

    void OnWaveStart()
    {
        
    }

    void OnWaveEnd()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (WaveManager.Singleton != null)
        {
            WaveManager.Singleton.onWaveStart -= OnWaveStart;
            WaveManager.Singleton.onWaveFinished -= OnWaveEnd;
        }
    }
}
