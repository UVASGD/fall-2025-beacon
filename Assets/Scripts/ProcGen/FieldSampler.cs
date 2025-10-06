using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Beacon.ProcGen {
    public static class FieldSampler
    {
        static float HashLattice(ulong seed, int3 p) {
            ulong h = Hash.Hash64(seed, p.x, p.y, p.z);
            return (float)Hash.ToUnit01(h);
        }

        static float Smooth(float t) => t * t * (3f - 2f * t);

        // 3D value noise
        public static float Value3D(ulong seed, float3 p) {
            int3 i = (int3)floor(p);
            float3 f = frac(p);

            float c000 = HashLattice(seed, i + int3(0, 0, 0));
            float c100 = HashLattice(seed, i + int3(1, 0, 0));
            float c010 = HashLattice(seed, i + int3(0, 1, 0));
            float c110 = HashLattice(seed, i + int3(1, 1, 0));
            float c001 = HashLattice(seed, i + int3(0, 0, 1));
            float c101 = HashLattice(seed, i + int3(1, 0, 1));
            float c011 = HashLattice(seed, i + int3(0, 1, 1));
            float c111 = HashLattice(seed, i + int3(1, 1, 1));

            float tx = Smooth(f.x), ty = Smooth(f.y), tz = Smooth(f.z);

            float x00 = lerp(c000, c100, tx);
            float x10 = lerp(c010, c110, tx);
            float x01 = lerp(c001, c101, tx);
            float x11 = lerp(c011, c111, tx);

            float y0 = lerp(x00, x10, ty);
            float y1 = lerp(x01, x11, ty);

            return lerp(y0, y1, tz);
        }

        // Fractional Brownian Motion
        public static float Fbm3D(ulong seed, float3 p, int octaves = 5, float lacun = 2f, float gain = 0.5f) {
            float a = 0f, amp = 0.5f, freq = 1f;
            for (int o = 0; o < octaves; o++)
            {
                a += (Value3D(seed + (uint)o * 1013904223u, p * freq) * 2f - 1f) * amp;
                freq *= lacun;
                amp *= gain;
            }
            return saturate(a * 0.5f + 0.5f);
        }
    }
}
