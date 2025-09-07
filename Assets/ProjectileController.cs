using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;

    private float travelDistance;
    
    public void Initialize(Transform _target, float _speed, float _damage)
    {
        target = _target;
        speed = _speed;
        damage = _damage;
    }

    private void FixedUpdate()
    {
        //if(travelDistance > range)
        //    Destroy(gameObject);
        if(target == null)
        {
            Destroy(gameObject);
            return;
        }


        Vector3 directionToTarget = target.position - transform.position;
        float distanceToTarget = directionToTarget.magnitude;
        transform.rotation = Quaternion.LookRotation(directionToTarget);
        transform.position += transform.forward * Mathf.Min(speed * Time.fixedDeltaTime, distanceToTarget);
        
        if(Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            //Hit
            target.GetComponent<IHealth>().ChangeHealth(-damage);
            Destroy(gameObject);
        }
        else
        {
            travelDistance += Mathf.Min(speed * Time.fixedDeltaTime, distanceToTarget);
        }
    }
}
