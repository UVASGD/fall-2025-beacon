using System;
using System.Text;

namespace Beacon.ProcGen
{

    /// <summary>
    /// 64-bit FNV-1a hashing with an avalanche finalizer.
    /// Overloads mix a seed with strings/ints for tuple-style hashing in procgen.
    /// </summary>

    public static class Hash
    {
        /// <summary>
        /// Hash a seed and string into a 64-bit value.
        /// Other overloads add ints for tuples (e.g., ids, channels).
        /// </summary>
        /// <param name="seed">Base 64-bit seed.</param>
        /// <param name="s">String to mix.</param>
        /// <returns>64-bit hash suitable for RNG derivation.</returns>

        public static ulong Hash64(ulong seed, string s)
        {
            ulong h = 1469598103934665603UL ^ seed;
            foreach (char c in s)
            {
                h ^= c;
                h *= 1099511628211UL;
            }
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, string s, long a)
        {
            ulong h = Hash64(seed, s);
            h ^= (ulong)a * 0x9E3779B185EBCA87UL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, string s, long a, int b)
        {
            ulong h = Hash64(seed, s, a);
            h ^= (uint)b * 0x85EBCA6B;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x)
        {
            ulong h = seed ^ (uint)x * 0x9E3779B185EBCA87UL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x, int y)
        {
            ulong h = Hash64(seed, x);
            h ^= (uint)y * 0xC2B2AE3D27D4EB4FUL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x, int y, int z)
        {
            ulong h = Hash64(seed, x, y);
            h ^= (uint)z * 0x165667B19E3779F9UL;
            return Avalanche(h);
        }

        /// <summary>
        /// Map a 64-bit hash to a uniform double in [0,1).
        /// Uses the top 53 bits for IEEE754 precision.
        /// </summary>
        /// <param name="h">64-bit hash.</param>
        /// <returns>Double in [0,1).</returns>

        public static double ToUnit01(ulong h) => ((h >> 11) & ((1UL << 53) - 1)) * (1.0 / (1UL << 53));

        static ulong Avalanche(ulong x)
        {
            x ^= x >> 33; x *= 0xff51afd7ed558ccdUL;
            x ^= x >> 33; x *= 0xc4ceb9fe1a85ec53UL;
            x ^= x >> 33; return x;
        }
    }
}
