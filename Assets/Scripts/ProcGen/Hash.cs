using System;
using System.Text;

namespace Beacon.ProcGen {
    // 64-bit FNV-1a with an avalanche finalizer
    // https://mojoauth.com/hashing/fnv-1a-in-c/
    public static class Hash {
        public static ulong Hash64(ulong seed, string s) {
            ulong h = 1469598103934665603UL ^ seed;
            foreach (char c in s) {
                h ^= c;
                h *= 1099511628211UL;
            }
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, string s, long a) {
            ulong h = Hash64(seed, s);
            h ^= (ulong)a * 0x9E3779B185EBCA87UL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, string s, long a, int b) {
            ulong h = Hash64(seed, s, a);
            h ^= (uint)b * 0x85EBCA6B;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x) {
            ulong h = seed ^ (uint)x * 0x9E3779B185EBCA87UL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x, int y) {
            ulong h = Hash64(seed, x);
            h ^= (uint)y * 0xC2B2AE3D27D4EB4FUL;
            return Avalanche(h);
        }

        public static ulong Hash64(ulong seed, int x, int y, int z) {
            ulong h = Hash64(seed, x, y);
            h ^= (uint)z * 0x165667B19E3779F9UL;
            return Avalanche(h);
        }

        public static double ToUnit01(ulong h) => ((h >> 11) & ((1UL << 53) - 1)) * (1.0 / (1UL << 53));

        static ulong Avalanche(ulong x) {
            x ^= x >> 33; x *= 0xff51afd7ed558ccdUL;
            x ^= x >> 33; x *= 0xc4ceb9fe1a85ec53UL;
            x ^= x >> 33; return x;
        }
    }
}