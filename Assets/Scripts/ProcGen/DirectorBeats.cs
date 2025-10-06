using UnityEngine;

namespace Beacon.ProcGen {
    public static class DirectorBeats
    {
        // Example: every 8s we draw a reproducible intensity tweak in [0,1)
        public static float BeatAdjust(string versionedPath, int beatIndex)
        {
            ulong sub = SeedRegistry.SubSeed($"Director/Beats@{versionedPath}");
            double u = DetRng.Uniform01(sub, "Beat", beatIndex, 0);
            return (float)u;
        }

        // Example policy: convert tweak  spawn budget multiplier
        public static float BudgetMultiplier(int beatIndex)
        {
            float t = BeatAdjust("v1", beatIndex);
            // center around 1.0 with 20% swing
            return 0.9f + 0.2f * t;
        }

        public static int BeatIndex(float timeSeconds, float bucketSeconds = 8f)
            => Mathf.FloorToInt(timeSeconds / bucketSeconds);
    }
}