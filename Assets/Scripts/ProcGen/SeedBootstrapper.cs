using UnityEngine;

namespace Beacon.ProcGen
{
    /// <summary>
    /// Unity component that chooses the run seed at startup.
    /// Supports fixed seed, persistent auto-generated seed (PlayerPrefs), and manual rerolls.
    /// </summary>

    [DefaultExecutionOrder(-10000)]
    [DisallowMultipleComponent]
    public class SeedBootstrapper : MonoBehaviour
    {
        [Header("Seed Selection")]
        [Tooltip("If true, use the Set Seed field below. If false, auto-generate a seed.")]
        public bool useSetSeed = true;

        [Tooltip("Used only when 'Use Set Seed' is true.")]
        public ulong setSeed = 123456789UL;

        [Header("Persistence (optional)")]
        [Tooltip("If true and Use Set Seed is false, we persist the generated seed across play sessions.")]
        public bool persistGeneratedSeed = false;

        [Tooltip("PlayerPrefs key for persisting the generated seed (as hex string).")]
        public string playerPrefsKey = "Beacon.RunSeed";

        /// <summary>
        /// Generate a fresh seed when not using a fixed seed.
        /// Optionally persist to PlayerPrefs and set SeedRegistry.RunSeed.
        /// </summary>
        /// <returns>None.</returns>

        [ContextMenu("Reroll Seed (only when not using set seed)")]
        public void RerollSeed()
        {
            if (useSetSeed) return;
            ulong s = SeedRegistry.GenerateRandomSeed();
            if (persistGeneratedSeed)
                PlayerPrefs.SetString(playerPrefsKey, s.ToString("X16"));
            SeedRegistry.SetRunSeed(s);
            Debug.Log($"[SeedBootstrapper] Rerolled RunSeed = 0x{s:X16}");
        }

        /// <summary>
        /// Pick the final run seed based on inspector settings.
        /// Persists auto-generated seeds when enabled and initializes SeedRegistry.
        /// </summary>
        /// <returns>None.</returns>

        private void Awake()
        {
            ulong finalSeed;

            if (useSetSeed)
            {
                finalSeed = setSeed;
            }
            else if (persistGeneratedSeed && PlayerPrefs.HasKey(playerPrefsKey))
            {
                string hex = PlayerPrefs.GetString(playerPrefsKey);
                if (ulong.TryParse(hex, System.Globalization.NumberStyles.HexNumber, null, out var saved))
                    finalSeed = saved;
                else
                    finalSeed = SeedRegistry.GenerateRandomSeed();
            }
            else
            {
                finalSeed = SeedRegistry.GenerateRandomSeed();
                if (persistGeneratedSeed)
                    PlayerPrefs.SetString(playerPrefsKey, finalSeed.ToString("X16"));
            }

            SeedRegistry.SetRunSeed(finalSeed);
            Debug.Log($"[SeedBootstrapper] RunSeed = 0x{finalSeed:X16} (useSetSeed={useSetSeed}, persisted={persistGeneratedSeed})");
        }
    }
}
