using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloomCannonController : MonoBehaviour
{
    // Data describing each bloom stage (indexed by level)
    // Includes: damage, range, and sprite to activate at that stage.
    [System.Serializable]
    public struct BloomLevelData
    {
        public float damage;
        public float range;
        public GameObject sprite;
    }

    // Ordered list of levels. Index = stage number.
    // Expected: levels[0] = unused or level 0 placeholder
    //           levels[1] = stage 1, levels[2] = stage 2, levels[3] = stage 3
    public BloomLevelData[] levels;

    // Current upgrade stage of the cannon. Starts at level 1.
    private int currentLevel = 1;

    // Number of waves required to grow from one bloom stage to the next.
    private int baseGrowTime = 3;

    // Remaining waves until next bloom.
    private int growTimeLeft;

    // Cached component references for performance.
    private DamageController damageController;
    private TurretTargetingController turretTargetingController;
    private CannonController cannonController;

    private void Awake()
    {
        // Validate correct number of level entries.
        // For this cannon, we expect exactly 3 bloom levels.
        if (levels.Length != 3)
        {
            Debug.LogError("BloomCannonController: Level data array size must be 3!");
        }

        // Initialize growth timer.
        growTimeLeft = baseGrowTime;

        // Cache frequently accessed components.
        damageController = GetComponent<DamageController>();
        turretTargetingController = GetComponent<TurretTargetingController>();
        cannonController = GetComponent<CannonController>();
    }

    private void OnEnable()
    {
        // Subscribe to wave completion event.
        // Each completed wave progresses bloom growth.
        WaveManager.Singleton.onWaveFinished += GrowALittle;
    }

    /// <summary>
    /// Called when a wave ends. Reduces the time required for the next bloom.
    /// Stops doing anything once max level is reached.
    /// </summary>
    private void GrowALittle()
    {
        // Already fully bloomed—no further growth possible.
        if (currentLevel == 3)
            return;

        // Progress toward next bloom stage.
        growTimeLeft--;

        // If timer runs out, upgrade.
        if (growTimeLeft <= 0)
        {
            Bloom();
        }
    }

    /// <summary>
    /// Increases the cannon's bloom level, updates stats, and refreshes sprite visuals.
    /// Each bloom requires slightly more waves than the last.
    /// </summary>
    private void Bloom()
    {
        currentLevel++;

        // Increase growth time for next stage (makes higher levels slower to reach).
        growTimeLeft = baseGrowTime + currentLevel;

        // Apply new stats from this bloom stage.
        damageController.damage = levels[currentLevel - 1].damage;
        turretTargetingController.rangeRadius = levels[currentLevel - 1].range;
        cannonController.range = levels[currentLevel - 1].range;

        // Refresh displayed sprite to match current bloom level.
        UpdateSprite();
    }

    /// <summary>
    /// Enables only the sprite for the current bloom stage and disables all others.
    /// </summary>
    private void UpdateSprite()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            // Activate only the sprite that matches the current level index.
            bool shouldShow = i == currentLevel;
            levels[i].sprite.SetActive(shouldShow);
        }
    }

    private void OnDisable()
    {
        // Unsubscribe from event to prevent memory leaks or null references
        // if this object is destroyed or disabled mid-game.
        WaveManager.Singleton.onWaveFinished -= GrowALittle;
    }
}
