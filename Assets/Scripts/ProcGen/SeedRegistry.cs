using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Beacon.ProcGen
{
    /// <summary>
    /// Central source of truth for the master run seed and derivation helpers.
    /// Use SubSeed(path[, scope]) to carve stable, independent sub-seeds per system.
    /// </summary>

    public static class SeedRegistry
    {

        /// <summary>
        /// Current master/run seed for this session.
        /// </summary>
        /// <returns>64-bit run seed.</returns>

        public static ulong RunSeed { get; private set; } = 0xDEADBEEFCAFEBABEUL;

        /// <summary>
        /// Set/override the master run seed; call early during bootstrapping.
        /// </summary>
        /// <param name="seed">New run seed.</param>
        /// <returns>None.</returns>

        public static void SetRunSeed(ulong seed) => RunSeed = seed;

        /// <summary>
        /// Create a cryptographically strong random 64-bit seed.
        /// </summary>
        /// <returns>New random seed.</returns>

        public static ulong GenerateRandomSeed()
        {
            Span<byte> buf = stackalloc byte[8];
            RandomNumberGenerator.Fill(buf);
            return BitConverter.ToUInt64(buf);
        }

        /// <summary>
        /// Create a non-cryptographic time-based seed.
        /// Mixes UTC ticks, Stopwatch, and TickCount for variability.
        /// </summary>
        /// <returns>New time-derived seed.</returns>

        public static ulong GenerateTimeSeed()
        {
            unchecked
            {
                ulong a = (ulong)DateTime.UtcNow.Ticks;
                ulong b = (ulong)Stopwatch.GetTimestamp();
                ulong c = (ulong)(uint)Environment.TickCount;

                ulong x = a;
                x ^= (b + 0x9E3779B97F4A7C15UL) + (x << 6) + (x >> 2);
                x ^= (c + 0x9E3779B97F4A7C15UL) + (x << 6) + (x >> 2);
                return x;
            }
        }

        /// <summary>
        /// Derive a stable sub-seed from RunSeed and a string path.
        /// </summary>
        /// <param name="path">Namespaced path (e.g., "Map/Chests").</param>
        /// <returns>Derived 64-bit sub-seed.</returns>

        public static ulong SubSeed(string path) => Hash.Hash64(RunSeed, path);

        /// <summary>
        /// Deeper derivation that also mixes an integral scope (e.g., level, chunk).
        /// </summary>
        /// <param name="path">Namespaced path.</param>
        /// <param name="scope">Additional scope value.</param>
        /// <returns>Derived 64-bit sub-seed.</returns>

        public static ulong SubSeed(string path, long scope) => Hash.Hash64(RunSeed, path, scope);
    }
}
