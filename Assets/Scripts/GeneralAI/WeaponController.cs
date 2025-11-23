using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Transform target;
    public float speed = 4f;
    public float damage = 3f; //This is for each fire point!!
    public float fireRate = 0.7f;

    public List<Transform> firePoints = new List<Transform>();
    public GameObject projectilePrefab;

    private float cooldown = 0f;
    private bool enableShooting;

    private CarrierBuffController carrierBuffController;

    private void Awake()
    {
        if(GetComponent<CarrierBuffController>() != null)
            carrierBuffController = GetComponent<CarrierBuffController>();
    }

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime * GlobalSettings.i.TimeScale;
            return;
        }

        if (target == null)
            return;

        HandleShootingTarget();
    }

    public void SetEnableShooting(bool setTo)
    {
        enableShooting = setTo;
    }

    private void HandleShootingTarget()
    {
        if (target == null)
            return;

        if (enableShooting)
            FireProjectiles();
    }

    private void FireProjectiles() //Instantites and encodes a projectile at each firePoint. Encodes projectile stats and target to home to.
    {
        foreach(Transform firePoint in firePoints)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            newProjectile.GetComponent<ProjectileController>().Initialize(target, speed, GetDamage());
        }

        cooldown = 1 / fireRate;
    }

    private float GetDamage()
    {
        if(carrierBuffController != null && carrierBuffController.CarrierInRange())
        {
            return damage * (1f + carrierBuffController.damageBuff);
        }
        else
        {
            return damage;
        }
    }
}
