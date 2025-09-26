using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CannonController : MonoBehaviour
{
    public TurretTargetingController targetingController;
    public float range = 20f;
    public float speed = 6f;
    public float fireRate = 1f;

    public Transform turretHead;
    public List<Transform> firePoints = new List<Transform>();
    public GameObject projectilePrefab;

    public LevelController levelController;
    public DamageController damageController;   

    private Transform target;
    private float cooldown = 0f;

    private void Awake()
    {
        if(levelController == null)
            levelController = GetComponent<LevelController>();
        levelController.onLevelUp += OnLevelUp;
    }

    private void Update()
    {
        if (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            return;
        }

        if (targetingController == null || targetingController.GetTarget() == null)
            return;

        target = targetingController.GetTarget();
        HandleShootingTarget();
    }

    private void HandleShootingTarget()
    {
        if (target == null)
            return;

        Vector3 direction = target.position - turretHead.position;
        direction.y = 0;
        turretHead.rotation = Quaternion.LookRotation(direction);

        float effectiveRange = range;
        if (RelicManager.Singleton != null)
            effectiveRange = RelicManager.Singleton.Apply(range, StatType.Range);

        if (Vector3.Distance(target.position, transform.position) < effectiveRange + 0.01f)
            FireProjectiles();
    }

    private void FireProjectiles() //Instantites and encodes a projectile at each firePoint. Encodes projectile stats and target to home to.
    {
        float effSpeed = speed;
        float effDamage = damageController.damage;
        float effFireRate = fireRate;

        if (RelicManager.Singleton != null)
        {
            effSpeed = RelicManager.Singleton.Apply(speed, StatType.ProjectileSpeed);
            effDamage = RelicManager.Singleton.Apply(damageController.damage, StatType.Damage);
            effFireRate = RelicManager.Singleton.Apply(fireRate, StatType.FireRate);
        }

        foreach (Transform firePoint in firePoints)
        {
            GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            newProjectile.GetComponent<ProjectileController>().Initialize(target, effSpeed, effDamage);
        }

        cooldown = 1f / Mathf.Max(0.0001f, effFireRate);
    }

    private void OnLevelUp()
    {
        int currentLevel = levelController.GetLevel();
        damageController.damage *= 1f+(1f / (currentLevel - 1));
        fireRate *= 1f+(1f / (currentLevel - 1));
    }
}
