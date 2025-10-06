using UnityEngine;

namespace Beacon.ProcGen
{
    /// <summary>
    /// Helpers to convert time into reproducible "beats" for encounter pacing.
    /// Built on SeedRegistry + DetRng for stable, versionable knobs.
    /// </summary>

    public static class DirectorBeats
    {

        /// <summary>
        /// Reproducible tweak in [0,1) for a given versioned path and beat index.
        /// Change 'versionedPath' to roll out new balancing without breaking old runs.
        /// </summary>
        /// <param name="versionedPath">Namespaced, versioned string (e.g., "Director/Beats@v1").</param>
        /// <param name="beatIndex">Discrete beat bucket index.</param>
        /// <returns>A float in [0,1).</returns>

        public static float BeatAdjust(string versionedPath, int beatIndex)
        {
            ulong sub = SeedRegistry.SubSeed($"Director/Beats@{versionedPath}");
            double u = DetRng.Uniform01(sub, "Beat", beatIndex, 0);
            return (float)u;
        }

        /// <summary>
        /// Example policy mapping the tweak to a spawn-budget multiplier centered near 1.0.
        /// </summary>
        /// <param name="beatIndex">Beat index (see BeatIndex).</param>
        /// <returns>Budget multiplier for that beat.</returns>

        public static float BudgetMultiplier(int beatIndex)
        {
            float t = BeatAdjust("v1", beatIndex);

            return 0.9f + 0.2f * t;
        }

        /// <summary>
        /// Quantize time into fixed-size beat buckets.
        /// </summary>
        /// <param name="timeSeconds">Elapsed time in seconds.</param>
        /// <param name="bucketSeconds">Beat length in seconds (default 8).</param>
        /// <returns>Floor(time / bucketSeconds).</returns>

        public static int BeatIndex(float timeSeconds, float bucketSeconds = 8f)
            => Mathf.FloorToInt(timeSeconds / bucketSeconds);
    }
}
