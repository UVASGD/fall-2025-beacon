using UnityEngine;
using Unity.Mathematics;

namespace Beacon.ProcGen {
    public class ProcgenSmokeTest : MonoBehaviour {
        [Header("Optional Override for Testing")]
        public bool overrideSeed = false;
        public ulong inspectorSeed = 420698008UL;

        void Start() {
            // Only override when explicitly requested
            if (overrideSeed)
                SeedRegistry.SetRunSeed(inspectorSeed);

            // Read the seed chosen by the bootstrapper (or the override)
            ulong runSeed = SeedRegistry.RunSeed;
            Debug.Log($"[SmokeTest] Using RunSeed = 0x{runSeed:X16}");

            // Derive sub-seeds
            ulong worldSeed = SeedRegistry.SubSeed("World@v1");
            ulong spawnsSeed = SeedRegistry.SubSeed("Spawns@v1");
            ulong relicsSeed = SeedRegistry.SubSeed("Relics/Offers@v1");
            ulong directorSeed = SeedRegistry.SubSeed("Director/Beats@v1");
            Debug.Log($"[SmokeTest] SubSeeds: World={worldSeed} Spawns={spawnsSeed} Relics={relicsSeed} Director={directorSeed}");

            // Simple demo rolls that depend on those sub-seeds
            long sectorId = 203;
            int waveIdx = 5;

            // pack IDs with explicit unsigned widths
            ulong laneKey = ((ulong)sectorId << 16) | (uint)waveIdx;
            double lanePick = DetRng.Uniform01(spawnsSeed, "Lane", (long)laneKey, 0);
            Debug.Log($"[SmokeTest] lanePick={lanePick:F6}");

            var waveRng = DetRng.FromContext(spawnsSeed, "Wave", sectorId, waveIdx);
            int count = waveRng.Range(3, 6);
            Debug.Log($"[SmokeTest] wave count={count}");
            for (int i = 0; i < count; i++)
            {
                float t = (float)(1.0 + 3.0 * waveRng.NextDouble01());
                int lane = waveRng.Range(0, 3);
                Debug.Log($"  - spawn #{i}: t={t:F2}s lane={lane}");
            }

            ulong oreSeed = SeedRegistry.SubSeed("Resources/Ore/Iron@v3");
            for (int x = 0; x <= 3; x++)
            {
                float3 pos = new float3(x * 25f, 0f, 50f);
                float d = FieldSampler.Fbm3D(oreSeed, pos * 0.01f, 5, 2f, 0.5f);
                Debug.Log($"[SmokeTest] ore@{pos} = {d:F3}");
            }
        }
    }
}
