using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    private Transform target;
    private float speed;
    private float damage;
    private IHealth returnDamageHealth;

    private float travelDistance;
    
    public void Initialize(Transform _target, float _speed, float _damage, IHealth _returnDamageHealth = null)
    {
        target = _target;
        speed = _speed;
        damage = _damage;
        returnDamageHealth = _returnDamageHealth;
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
            float returnDam = target.GetComponent<IHealth>().ChangeHealth(-damage);
            if(returnDamageHealth != null)
                returnDamageHealth.ChangeHealth(-returnDam);
            Destroy(gameObject);
        }
        else
        {
            travelDistance += Mathf.Min(speed * Time.fixedDeltaTime, distanceToTarget);
        }
    }
}
