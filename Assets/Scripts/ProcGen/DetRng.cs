using System;
using System.Runtime.CompilerServices;

namespace Beacon.ProcGen
{

    /// <summary>
    /// Deterministic RNG building blocks for procgen.
    /// Combines a PCG32 stream RNG (stateful) with stateless hash-based helpers.
    /// Use SeedRegistry.SubSeed(...) to derive per-system seeds.
    /// Streams give reproducible sequences; hashed helpers give random-access values.
    /// </summary>

    public struct DetRng
    {
        private ulong _state;
        private readonly ulong _inc;

        /// <summary>
        /// Initialize a PCG32 stream RNG.
        /// The optional 'stream' selects an independent sequence for the same seed.
        /// </summary>
        /// <param name="seed">Base seed for the stream.</param>
        /// <param name="stream">Sequence selector (must be odd internally).</param>

        public DetRng(ulong seed, ulong stream = 1442695040888963407UL)
        {
            _state = 0UL;
            _inc = (stream << 1) | 1UL;
            NextUInt();
            _state += seed + 0x9E3779B97F4A7C15UL;
            NextUInt();
        }

        /// <summary>
        /// Derive a stream RNG from a sub-seed and a context tuple.
        /// Same (subSeed, ns, a, b) => same sequence; different context => independent.
        /// </summary>
        /// <param name="subSeed">Module-level seed (e.g., SeedRegistry.SubSeed).</param>
        /// <param name="ns">Namespace string to separate usages.</param>
        /// <param name="a">Optional context value.</param>
        /// <param name="b">Optional context value.</param>
        /// <returns>A deterministic stream RNG for that context.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DetRng FromContext(ulong subSeed, string ns, long a = 0, int b = 0)
        {

            ulong stream = (Hash.Hash64(subSeed, ns, a, b) << 1) | 1UL;
            return new DetRng(subSeed, stream);
        }

        /// <summary>
        /// Stateless uniform double in [0,1) from a hashed (subSeed, ns, id, channel) tuple.
        /// Use for per-entity randoms without storing RNG state.
        /// </summary>
        /// <param name="subSeed">Module sub-seed.</param>
        /// <param name="ns">Namespace string.</param>
        /// <param name="id">Stable object id.</param>
        /// <param name="channel">Extra decorrelation lane.</param>
        /// <returns>A reproducible uniform in [0,1).</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Uniform01(ulong subSeed, string ns, long id, int channel = 0)
            => Hash.ToUnit01(Hash.Hash64(subSeed, ns, id, channel));

        /// <summary>
        /// Stateless inclusive integer in [min,max] from the same tuple hashing.
        /// Swaps min/max if provided in reverse.
        /// </summary>
        /// <param name="subSeed">Module sub-seed.</param>
        /// <param name="ns">Namespace.</param>
        /// <param name="id">Stable object id.</param>
        /// <param name="min">Lower bound (inclusive).</param>
        /// <param name="max">Upper bound (inclusive).</param>
        /// <param name="channel">Extra lane.</param>
        /// <returns>Deterministic integer in [min,max].</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int RangeHashed(ulong subSeed, string ns, long id, int min, int max, int channel = 0)
        {
            if (max < min) { var t = min; min = max; max = t; }
            ulong h = Hash.Hash64(subSeed, ns, id, channel);
            uint span = (uint)((max - min) + 1);
            return (int)(min + (uint)h % span);
        }

        /// <summary>
        /// Next 32-bit unsigned integer from the PCG32 stream.
        /// </summary>
        /// <returns>A 32-bit pseudo-random value.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint NextUInt()
        {
            ulong old = _state;
            _state = old * 6364136223846793005UL + _inc;
            uint xorshifted = (uint)(((old >> 18) ^ old) >> 27);
            uint rot = (uint)(old >> 59);
            return (xorshifted >> (int)rot) | (xorshifted << (int)((-rot) & 31));
        }

        /// <summary>
        /// Uniform [0,1) from the stream with ~53 bits of precision (two draws).
        /// </summary>
        /// <returns>A double in [0,1).</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double NextDouble01()
        {
            ulong hi = (ulong)(NextUInt() & 0x001FFFFF);
            ulong lo = NextUInt();
            ulong frac53 = (hi << 32) | lo;
            return frac53 * (1.0 / (1UL << 53));
        }

        /// <summary>
        /// Uniform [0,1) from the stream as a float.
        /// </summary>
        /// <returns>A float in [0,1).</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float NextFloat01() => (float)NextDouble01();

        /// <summary>
        /// Inclusive integer in [min,max] using this RNG's stream state.
        /// Swaps bounds if provided in reverse.
        /// </summary>
        /// <param name="min">Lower bound (inclusive).</param>
        /// <param name="max">Upper bound (inclusive).</param>
        /// <returns>An integer in [min,max].</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Range(int min, int max)
        {
            if (max < min) { var t = min; min = max; max = t; }
            uint span = (uint)((max - min) + 1);
            return (int)(min + (NextUInt() % span));
        }

        /// <summary>
        /// Bernoulli trial on the stream.
        /// </summary>
        /// <param name="p">Success probability in [0,1].</param>
        /// <returns>true with probability p.</returns>

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Chance(double p) => NextDouble01() < p;

        /// <summary>
        /// Standard normal sample via Box–Muller.
        /// Mean 0, standard deviation 1.
        /// </summary>
        /// <returns>A Gaussian deviate.</returns>

        public double Gaussian()
        {
            double u1 = Math.Max(1e-12, NextDouble01());
            double u2 = NextDouble01();
            double r = Math.Sqrt(-2.0 * Math.Log(u1));
            double theta = 2.0 * Math.PI * u2;
            return r * Math.Cos(theta);
        }

        /// <summary>
        /// In-place Fisher–Yates shuffle using the stream.
        /// </summary>
        /// <param name="span">Span to shuffle.</param>

        public void Shuffle<T>(Span<T> span)
        {
            for (int i = span.Length - 1; i > 0; i--)
            {
                int j = (int)(NextUInt() % (uint)(i + 1));
                (span[i], span[j]) = (span[j], span[i]);
            }
        }

        /// <summary>
        /// Pick index proportional to non-negative weights using the stream.
        /// Returns the last index if the input is degenerate (sum <= 0).
        /// </summary>
        /// <param name="weights">Unnormalized weights (>=0).</param>
        /// <returns>Selected index.</returns>

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
