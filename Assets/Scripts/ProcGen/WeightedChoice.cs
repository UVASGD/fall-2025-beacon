using System;
using System.Collections.Generic;

namespace Beacon.ProcGen {
    public static class WeightedChoice {
        public static int PickIndex(double u01, IReadOnlyList<float> weights) {
            double sum = 0; foreach (var w in weights) sum += Math.Max(0, w);
            double t = u01 * sum;
            double acc = 0;
            for (int i = 0; i < weights.Count; i++) {
                acc += Math.Max(0, weights[i]);
                if (t < acc) return i;
            }
            return Math.Max(0, weights.Count - 1);
        }
    }
}
