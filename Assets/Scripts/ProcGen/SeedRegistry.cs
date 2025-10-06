using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace Beacon.ProcGen {
    public static class SeedRegistry {
        // Master/run seed (persistant)
        public static ulong RunSeed { get; private set; } = 0xDEADBEEFCAFEBABEUL;

        public static void SetRunSeed(ulong seed) => RunSeed = seed;

        public static ulong GenerateRandomSeed() {
            Span<byte> buf = stackalloc byte[8];
            RandomNumberGenerator.Fill(buf);
            return BitConverter.ToUInt64(buf);
        }

        public static ulong GenerateTimeSeed()
        {
            unchecked
            {
                ulong a = (ulong)DateTime.UtcNow.Ticks;           // 100ns precision
                ulong b = (ulong)Stopwatch.GetTimestamp();        // high-res monotonic
                ulong c = (ulong)(uint)Environment.TickCount;     // 32-bit ms since start (cast to uint to avoid sign-extension)

                ulong x = a;
                x ^= (b + 0x9E3779B97F4A7C15UL) + (x << 6) + (x >> 2);
                x ^= (c + 0x9E3779B97F4A7C15UL) + (x << 6) + (x >> 2);
                return x;
            }
        }
        /// Derive a stable sub-seed from run seed + path
        public static ulong SubSeed(string path) => Hash.Hash64(RunSeed, path);

        /// Derive a deeper sub-seed
        public static ulong SubSeed(string path, long scope) => Hash.Hash64(RunSeed, path, scope);
    }
}
