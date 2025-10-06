using System;
using System.Runtime.CompilerServices;

namespace Beacon.ProcGen
{
    /// Deterministic RNG:
    /// - Stream RNG (PCG32)
    /// - Stateless hash-per-ID helpers
    public struct DetRng
    {
        private ulong _state;        // PCG32 internal state
        private readonly ulong _inc; // must be odd

        /// Create a stream RNG. 'stream' selects an independent sequence for the same seed.
        public DetRng(ulong seed, ulong stream = 1442695040888963407UL)
        {
            _state = 0UL;
            _inc = (stream << 1) | 1UL; // force odd
            NextUInt();                   // PCG seeding ritual
            _state += seed + 0x9E3779B97F4A7C15UL;
            NextUInt();
        }

        /// Build a stream RNG derived from a module sub-seed and a context tuple.
        /// SAME inputs  SAME sequence; different context  independent sequence.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DetRng FromContext(ulong subSeed, string ns, long a = 0, int b = 0)
        {
            // Make sure we keep everything as ulong to avoid sign-extension issues.
            ulong stream = (Hash.Hash64(subSeed, ns, a, b) << 1) | 1UL; // ensure odd increment
            return new DetRng(subSeed, stream);
        }

        // -------- Stateless random-access (no stream/state) --------

        /// Uniform [0,1) from hashed ID tuple (random access).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Uniform01(ulong subSeed, string ns, long id, int channel = 0)
            => Hash.ToUnit01(Hash.Hash64(subSeed, ns, id, channel));

        /// Inclusive int in [min,max] via hashed ID tuple (random access).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeHashed(ulong subSeed, string ns, long id, int min, int max, int channel = 0)
        {
            if (max < min) { var t = min; min = max; max = t; }
            ulong h = Hash.Hash64(subSeed, ns, id, channel);
            uint span = (uint)((max - min) + 1);
            return (int)(min + (uint)h % span);
        }

        // ----------------- Stream RNG API (stateful) -----------------

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NextUInt()
        {
            ulong old = _state;
            _state = old * 6364136223846793005UL + _inc;
            uint xorshifted = (uint)(((old >> 18) ^ old) >> 27);
            uint rot = (uint)(old >> 59);
            return (xorshifted >> (int)rot) | (xorshifted << (int)((-rot) & 31));
        }

        /// Uniform [0,1) with 53-bit precision (uses two 32-bit draws).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble01()
        {
            ulong hi = (ulong)(NextUInt() & 0x001FFFFF); // 21 bits
            ulong lo = NextUInt();                       // 32 bits
            ulong frac53 = (hi << 32) | lo;              // 53-bit fraction
            return frac53 * (1.0 / (1UL << 53));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat01() => (float)NextDouble01();

        /// Inclusive integer in [min,max].
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            if (max < min) { var t = min; min = max; max = t; }
            uint span = (uint)((max - min) + 1);
            return (int)(min + (NextUInt() % span));
        }

        /// True with probability p (0..1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Chance(double p) => NextDouble01() < p;

        /// Standard normal via BoxMuller (mean 0, stddev 1).
        public double Gaussian()
        {
            double u1 = Math.Max(1e-12, NextDouble01());
            double u2 = NextDouble01();
            double r = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = 2.0 * Math.PI * u2;
            return r * Math.Cos(theta);
        }

        /// FisherYates shuffle in-place.
        public void Shuffle<T>(Span<T> span)
        {
            for (int i = span.Length - 1; i > 0; i--)
            {
                int j = (int)(NextUInt() % (uint)(i + 1));
                (span[i], span[j]) = (span[j], span[i]);
            }
        }

        /// Pick an index by unnormalized weights (>=0). Returns last index on degenerate input.
        public int PickWeighted(ReadOnlySpan<float> weights)
        {
            double sum = 0;
            for (int i = 0; i < weights.Length; i++) sum += Math.Max(0f, weights[i]);
            if (sum <= 0) return Math.Max(0, weights.Length - 1);
            double t = NextDouble01() * sum;
            double acc = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                acc += Math.Max(0f, weights[i]);
                if (t < acc) return i;
            }
            return Math.Max(0, weights.Length - 1);
        }
    }
}
