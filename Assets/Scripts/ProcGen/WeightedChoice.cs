using System;
using System.Collections.Generic;

namespace Beacon.ProcGen
{
    /// <summary>
    /// Deterministic weighted-choice helper.
    /// Use with a supplied uniform u01 for reproducible selections.
    /// </summary>

    public static class WeightedChoice
    {
        /// <summary>
        /// Select an index proportional to non-negative weights.
        /// If the sum is <= 0, returns the last index as a fallback.
        /// </summary>
        /// <param name="u01">Uniform random in [0,1).</param>
        /// <param name="weights">Unnormalized non-negative weights.</param>
        /// <returns>Chosen index.</returns>

        public static int PickIndex(double u01, IReadOnlyList<float> weights)
        {
            double sum = 0; foreach (var w in weights) sum += Math.Max(0, w);
            double t = u01 * sum;
            double acc = 0;
            for (int i = 0; i < weights.Count; i++)
            {
                acc += Math.Max(0, weights[i]);
                if (t < acc) return i;
            }
            return Math.Max(0, weights.Count - 1);
        }
    }
}
